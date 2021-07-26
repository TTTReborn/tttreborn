using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class Scoreboard : Panel
    {
        public static Scoreboard Instance;

        private readonly Dictionary<int, ScoreboardEntry> _entries = new();
        //TODO: Event on start of PreRound =>
        //Make all Entries trigger the Entry.UpdateForm()

        private readonly Dictionary<string, ScoreboardGroup> _scoreboardGroups = new();

        private readonly Header _header;

        private TableHeader _tableHeader;

        private readonly Panel _mainContent;

        private Panel _footer;

        public Scoreboard()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/scoreboard/Scoreboard.scss");

            _header = new Header(this);
            _tableHeader = new TableHeader(this);

            _mainContent = Add.Panel("mainContent");

            foreach (DefaultScoreboardGroup defaultScoreboardGroup in Enum.GetValues(typeof(DefaultScoreboardGroup)))
            {
                AddScoreboardGroup(defaultScoreboardGroup.ToString());
            }

            PlayerScore.OnPlayerAdded += AddPlayer;
            PlayerScore.OnPlayerUpdated += UpdatePlayer;
            PlayerScore.OnPlayerRemoved += (entry) => {
                RemovePlayer(entry);

                UpdateScoreboardGroups();
            };

            _footer = Add.Panel("footer");

            foreach (PlayerScore.Entry player in PlayerScore.All)
            {
                AddPlayer(player);
            }

            UpdateScoreboardGroups();
        }

        [Event("tttreborn.player.spawned")]
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
            _header.UpdateServerInfo();
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

            SetClass("open", Input.Down(InputButton.Score));
        }
    }
}
