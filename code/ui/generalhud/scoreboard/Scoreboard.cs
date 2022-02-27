using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Events;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class Scoreboard : Panel
    {
        public enum DefaultScoreboardGroup
        {
            Alive,
            Missing,
            Dead,
            Spectator
        }

        public static Scoreboard Instance;

        private readonly Dictionary<long, ScoreboardEntry> _entries = new();
        private readonly Dictionary<string, ScoreboardGroup> _scoreboardGroups = new();
        private readonly Dictionary<long, bool> _forcedSpecList = new();

        private readonly Panel _backgroundPanel;
        private readonly Panel _scoreboardContainer;
        private readonly ScoreboardHeader _scoreboardHeader;
        private readonly Panel _scoreboardContent;
        private readonly Panel _scoreboardFooter;

        public Scoreboard() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/scoreboard/Scoreboard.scss");

            _backgroundPanel = new(this);
            _backgroundPanel.AddClass("background-color-secondary");
            _backgroundPanel.AddClass("opacity-medium");
            _backgroundPanel.AddClass("fullscreen");

            _scoreboardContainer = new(this);
            _scoreboardContainer.AddClass("rounded");
            _scoreboardContainer.AddClass("scoreboard-container");

            _scoreboardHeader = new(_scoreboardContainer);
            _scoreboardHeader.AddClass("background-color-secondary");
            _scoreboardHeader.AddClass("opacity-heavy");
            _scoreboardHeader.AddClass("rounded-top");

            _scoreboardContent = new(_scoreboardContainer);
            _scoreboardContent.AddClass("background-color-primary");
            _scoreboardContent.AddClass("scoreboard-content");
            _scoreboardContent.AddClass("opacity-heavy");

            _scoreboardFooter = new(_scoreboardContainer);
            _scoreboardFooter.AddClass("background-color-secondary");
            _scoreboardFooter.AddClass("scoreboard-footer");
            _scoreboardFooter.AddClass("rounded-bottom");
            _scoreboardFooter.AddClass("opacity-heavy");

            Initialize();
        }

        [Event.Hotload]
        private void Initialize()
        {
            if (Host.IsServer)
            {
                return;
            }

            foreach (DefaultScoreboardGroup defaultScoreboardGroup in Enum.GetValues(typeof(DefaultScoreboardGroup)))
            {
                AddScoreboardGroup(defaultScoreboardGroup.ToString());
            }

            foreach (Client client in Client.All)
            {
                AddClient(client);
            }

            UpdateScoreboardGroups();
        }

        [Event(TTTEvent.Player.SPAWNED)]
        private void OnPlayerSpawned(TTTPlayer player)
        {
            UpdateClient(player.Client);
        }

        // unreliable currently due to S&Box issues
        // [Event(TTTEvent.Player.CONNECTED)]
        // public void OnPlayerConnected(Client client)
        // {
        //     AddClient(client);
        //     UpdateScoreboardGroups();
        // }

        // [Event(TTTEvent.Player.DISCONNECTED)]
        // private void OnPlayerDisconnected(long playerId, NetworkDisconnectionReason reason)
        // {
        //     RemoveClient(playerId);
        //     UpdateScoreboardGroups();
        // }

        public void AddClient(Client client)
        {
            if (client == null)
            {
                Log.Warning("Tried to add a client that isn't valid");

                return;
            }

            if (_entries.TryGetValue(client.PlayerId, out ScoreboardEntry _))
            {
                return;
            }

            ScoreboardGroup scoreboardGroup = GetScoreboardGroup(client);
            ScoreboardEntry scoreboardEntry = scoreboardGroup.AddEntry(client);

            scoreboardGroup.GroupMembers++;

            _entries.Add(client.PlayerId, scoreboardEntry);

            scoreboardGroup.UpdateLabel();
            _scoreboardHeader.UpdateServerInfo();
        }

        public void UpdateClient(Client client)
        {
            if (!_entries.TryGetValue(client.PlayerId, out ScoreboardEntry panel))
            {
                return;
            }

            ScoreboardGroup scoreboardGroup = GetScoreboardGroup(client);

            if (scoreboardGroup.GroupTitle != panel.ScoreboardGroupName)
            {
                // instead of remove and add, move the panel into the right parent
                RemoveClient(client.PlayerId);
                AddClient(client);
            }
            else
            {
                panel.Update();
            }

            UpdateScoreboardGroups();
        }

        public void Update()
        {
            foreach (Client client in Client.All)
            {
                UpdateClient(client);
            }
        }

        public void RemoveClient(long playerId)
        {
            if (!_entries.TryGetValue(playerId, out ScoreboardEntry panel))
            {
                return;
            }

            _scoreboardGroups.TryGetValue(panel.ScoreboardGroupName, out ScoreboardGroup scoreboardGroup);

            if (scoreboardGroup != null)
            {
                scoreboardGroup.GroupMembers--;
            }

            scoreboardGroup.UpdateLabel();

            panel.Delete();
            _entries.Remove(playerId);
        }

        public override void Tick()
        {
            base.Tick();

            if (!Input.Down(InputButton.Score))
            {
                SetClass("fade-in", false);

                return;
            }

            bool invalidList = false;

            foreach (Client client in Client.All)
            {
                if (!_entries.ContainsKey(client.PlayerId))
                {
                    invalidList = true;

                    break;
                }
            }

            if (!invalidList)
            {
                // Due to not having a `client.GetValue` change callback, we have to handle it differently
                foreach (Client client in Client.All)
                {
                    bool newIsForcedSpectator = client.GetValue<bool>("forcedspectator");

                    if (!_forcedSpecList.TryGetValue(client.PlayerId, out bool isForcedSpectator))
                    {
                        _forcedSpecList.Add(client.PlayerId, newIsForcedSpectator);
                    }
                    else if (isForcedSpectator != newIsForcedSpectator)
                    {
                        _forcedSpecList[client.PlayerId] = newIsForcedSpectator;

                        UpdateClient(client);
                    }
                }
            }
            else
            {
                foreach (ScoreboardEntry scoreboardEntry in _entries.Values)
                {
                    scoreboardEntry.Delete(true);
                }

                _entries.Clear();

                foreach (Client client in Client.All)
                {
                    AddClient(client);
                }

                UpdateScoreboardGroups();
            }

            SetClass("fade-in", true);
            _scoreboardContainer.SetClass("pop-in", true);
        }

        private ScoreboardGroup AddScoreboardGroup(string groupName)
        {
            if (_scoreboardGroups.ContainsKey(groupName))
            {
                return _scoreboardGroups[groupName];
            }

            ScoreboardGroup scoreboardGroup = new(_scoreboardContent, groupName);
            scoreboardGroup.UpdateLabel();

            _scoreboardGroups.Add(groupName, scoreboardGroup);

            return scoreboardGroup;
        }

        private ScoreboardGroup GetScoreboardGroup(Client client)
        {
            string group = DefaultScoreboardGroup.Alive.ToString();

            if (client.GetValue<bool>("forcedspectator"))
            {
                group = DefaultScoreboardGroup.Spectator.ToString();
            }
            else if (client.PlayerId != 0 && client.Pawn is TTTPlayer player)
            {
                if (player.IsConfirmed)
                {
                    group = DefaultScoreboardGroup.Dead.ToString();
                }
                else if (player.IsMissingInAction)
                {
                    group = DefaultScoreboardGroup.Missing.ToString();
                }
            }

            _scoreboardGroups.TryGetValue(group, out ScoreboardGroup scoreboardGroup);

            return scoreboardGroup ?? AddScoreboardGroup(group);
        }

        private void UpdateScoreboardGroups()
        {
            foreach (ScoreboardGroup value in _scoreboardGroups.Values)
            {
                value.Style.Display = value.GroupMembers == 0 ? DisplayMode.None : DisplayMode.Flex;
            }
        }
    }
}
