using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

#pragma warning disable IDE0051

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class Scoreboard : Panel
    {
        public enum DefaultScoreboardGroup
        {
            Alive,
            Missing,
            Dead,
            Spectator
        }

        public static Scoreboard Instance { get; set; }

        private readonly Dictionary<long, ScoreboardEntry> _entries = new();
        private readonly Dictionary<string, ScoreboardGroup> _scoreboardGroups = new();
        private readonly Dictionary<long, bool> _forcedSpecList = new();

        private ScoreboardHeader ScoreboardHeader { get; set; }
        private Panel ScoreboardContent { get; set; }
        private Panel ScoreboardFooter { get; set; }

        public Scoreboard()
        {
            Instance = this;

            Initialize();
        }

        private void Initialize()
        {
            if (Game.IsServer)
            {
                return;
            }

            foreach (DefaultScoreboardGroup defaultScoreboardGroup in Enum.GetValues(typeof(DefaultScoreboardGroup)))
            {
                AddScoreboardGroup(defaultScoreboardGroup.ToString());
            }

            foreach (IClient client in Game.Clients)
            {
                AddClient(client);
            }

            UpdateScoreboardGroups();
        }

        [Event("player_spawn")]
        protected void OnPlayerSpawned(Player player)
        {
            if (player == null || !player.IsValid)
            {
                return;
            }

            UpdateClient(player.Client);
        }

        [Event("player_connected")]
        public void OnPlayerConnected(IClient client)
        {
            AddClient(client);
            UpdateScoreboardGroups();
        }

        [Event("player_disconnected")]
        protected void OnPlayerDisconnected(long playerId, NetworkDisconnectionReason reason)
        {
            RemoveClient(playerId);
            UpdateScoreboardGroups();
        }

        public void AddClient(IClient client)
        {
            if (client == null)
            {
                Log.Warning("Tried to add a client that isn't valid");

                return;
            }

            if (_entries.TryGetValue(client.SteamId, out ScoreboardEntry _))
            {
                return;
            }

            ScoreboardGroup scoreboardGroup = GetScoreboardGroup(client);
            ScoreboardEntry scoreboardEntry = scoreboardGroup.AddEntry(client);

            scoreboardGroup.GroupMembers++;

            _entries.Add(client.SteamId, scoreboardEntry);

            scoreboardGroup.UpdateLabel();
            ScoreboardHeader.UpdateServerInfo();
        }

        public void UpdateClient(IClient client)
        {
            if (!_entries.TryGetValue(client.SteamId, out ScoreboardEntry panel))
            {
                return;
            }

            ScoreboardGroup scoreboardGroup = GetScoreboardGroup(client);

            if (scoreboardGroup.GroupTitle != panel.ScoreboardGroupName)
            {
                // instead of remove and add, move the panel into the right parent
                RemoveClient(client.SteamId);
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
            foreach (IClient client in Game.Clients)
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

            bool invalidList = _entries.Count != Game.Clients.Count;

            if (!invalidList)
            {
                foreach (IClient client in Game.Clients)
                {
                    if (!_entries.ContainsKey(client.SteamId))
                    {
                        invalidList = true;

                        break;
                    }
                }
            }

            if (!invalidList)
            {
                // Due to not having a `client.GetValue` change callback, we have to handle it differently
                foreach (IClient client in Game.Clients)
                {
                    bool newIsForcedSpectator = client.GetValue<bool>("forcedspectator");

                    if (!_forcedSpecList.TryGetValue(client.SteamId, out bool isForcedSpectator))
                    {
                        _forcedSpecList.Add(client.SteamId, newIsForcedSpectator);
                    }
                    else if (isForcedSpectator != newIsForcedSpectator)
                    {
                        _forcedSpecList[client.SteamId] = newIsForcedSpectator;

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

                foreach (IClient client in Game.Clients)
                {
                    AddClient(client);
                }

                UpdateScoreboardGroups();
            }
        }

        private ScoreboardGroup AddScoreboardGroup(string groupName)
        {
            if (_scoreboardGroups.ContainsKey(groupName))
            {
                return _scoreboardGroups[groupName];
            }

            ScoreboardGroup scoreboardGroup = new(ScoreboardContent, groupName);
            scoreboardGroup.UpdateLabel();

            _scoreboardGroups.Add(groupName, scoreboardGroup);

            return scoreboardGroup;
        }

        private ScoreboardGroup GetScoreboardGroup(IClient client)
        {
            string group = DefaultScoreboardGroup.Alive.ToString();

            if (client.GetValue<bool>("forcedspectator"))
            {
                group = DefaultScoreboardGroup.Spectator.ToString();
            }
            else if (client.SteamId != 0 && client.Pawn is Player player)
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
