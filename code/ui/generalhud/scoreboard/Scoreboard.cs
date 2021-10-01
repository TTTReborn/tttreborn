using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

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

        private readonly Dictionary<int, ScoreboardEntry> _entries = new();
        //TODO: Event on start of PreRound =>
        //Make all Entries trigger the Entry.UpdateForm()

        private readonly Dictionary<string, ScoreboardGroup> _scoreboardGroups = new();

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

            foreach (DefaultScoreboardGroup defaultScoreboardGroup in Enum.GetValues(typeof(DefaultScoreboardGroup)))
            {
                AddScoreboardGroup(defaultScoreboardGroup.ToString());
            }

            PlayerScore.OnPlayerAdded += AddPlayer;
            PlayerScore.OnPlayerUpdated += UpdatePlayer;
            PlayerScore.OnPlayerRemoved += (entry) =>
            {
                RemovePlayer(entry);

                UpdateScoreboardGroups();
            };

            foreach (PlayerScore.Entry player in PlayerScore.All)
            {
                AddPlayer(player);
            }

            UpdateScoreboardGroups();
        }

        [Event(TTTEvent.Player.Spawned)]
        private void OnPlayerSpawned(TTTPlayer player)
        {
            UpdatePlayer(player.GetClientOwner());
        }

        private void AddPlayer(PlayerScore.Entry entry)
        {
            ScoreboardGroup scoreboardGroup = GetScoreboardGroup(entry);
            ScoreboardEntry scoreboardEntry = scoreboardGroup.AddEntry(entry);

            scoreboardGroup.GroupMembers++;

            _entries.Add(entry.Id, scoreboardEntry);

            scoreboardGroup.UpdateLabel();
            _scoreboardHeader.UpdateServerInfo();
        }

        private void UpdatePlayer(PlayerScore.Entry entry)
        {
            if (!_entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
            {
                return;
            }

            ScoreboardGroup scoreboardGroup = GetScoreboardGroup(entry);

            if (scoreboardGroup.GroupTitle != panel.ScoreboardGroupName)
            {
                // instead of remove and add, move the panel into the right parent
                RemovePlayer(entry);
                AddPlayer(entry);
            }
            else
            {
                panel.UpdateFrom(entry);
            }

            UpdateScoreboardGroups();
        }

        public void UpdatePlayer(Client client)
        {
            foreach (PlayerScore.Entry entry in PlayerScore.All)
            {
                if (entry.Get<ulong>("steamid", 0) == client.SteamId)
                {
                    UpdatePlayer(entry);
                }
            }
        }

        public void Update()
        {
            foreach (PlayerScore.Entry entry in PlayerScore.All)
            {
                UpdatePlayer(entry);
            }
        }

        private void RemovePlayer(PlayerScore.Entry entry)
        {
            if (!_entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
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
            _entries.Remove(entry.Id);
        }

        public override void Tick()
        {
            base.Tick();

            SetClass("fade-in", Input.Down(InputButton.Score));
            _scoreboardContainer.SetClass("pop-in", Input.Down(InputButton.Score));
        }

        private ScoreboardGroup AddScoreboardGroup(string groupName)
        {
            if (_scoreboardGroups.ContainsKey(groupName))
            {
                return _scoreboardGroups[groupName];
            }

            ScoreboardGroup scoreboardGroup = new ScoreboardGroup(_scoreboardContent, groupName);
            scoreboardGroup.UpdateLabel();

            _scoreboardGroups.Add(groupName, scoreboardGroup);

            return scoreboardGroup;
        }

        private ScoreboardGroup GetScoreboardGroup(PlayerScore.Entry entry)
        {
            string group = DefaultScoreboardGroup.Alive.ToString();
            ulong steamId = entry.Get<ulong>("steamid", 0);

            if (entry.Get<bool>("forcedspectator", false))
            {
                group = DefaultScoreboardGroup.Spectator.ToString();
            }
            else if (steamId != 0)
            {
                foreach (Client client in Client.All)
                {
                    if (client.SteamId == steamId && client.Pawn != null)
                    {
                        if (client.Pawn is not TTTPlayer player)
                        {
                            break;
                        }

                        if (player.IsConfirmed)
                        {
                            group = DefaultScoreboardGroup.Dead.ToString();
                        }
                        else if (player.IsMissingInAction)
                        {
                            group = DefaultScoreboardGroup.Missing.ToString();
                        }

                        break;
                    }
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
                value.Style.Dirty();
            }
        }
    }
}
