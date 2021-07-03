using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class Scoreboard : Panel
    {
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
            StyleSheet.Load("/ui/Scoreboard.scss");

            _header = new Header(this);
            _tableHeader = new TableHeader(this);

            _mainContent = Add.Panel("mainContent");

            AddScoreboardGroup("Alive");

            PlayerScore.OnPlayerAdded += AddPlayer;
            PlayerScore.OnPlayerUpdated += UpdatePlayer;
            PlayerScore.OnPlayerRemoved += RemovePlayer;

            _footer = Add.Panel("footer");

            foreach (PlayerScore.Entry player in PlayerScore.All)
            {
                AddPlayer(player);
            }
        }

        private class Header : Panel
        {
            //public Label ServerDescription;

            private Panel _scoreboardLogo;
            private Label _serverName;
            private readonly Panel _informationHolder;
            private readonly Label _serverInfo;

            public Header(Panel parent)
            {
                Parent = parent;

                _scoreboardLogo = Add.Panel("scoreboardLogo");
                _informationHolder = Add.Panel("informationHolder");
                _serverName = _informationHolder.Add.Label("Trouble in Terry's Town", "serverName"); // Here will be the servername
                _serverInfo = _informationHolder.Add.Label("", "serverInfo");
                //ServerDescription = InformationHolder.Add.Label("This is the server description: Lorem ipsum dolor sit  elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat", "serverDescription");
            }

            public void UpdateServerInfo()
            {
                // TODO: Get this out of the header
                // TODO: Fill the other variables
                _serverInfo.Text = $"{PlayerScore.All.Length} Player(s) - Map: '{Sandbox.Global.MapName}'";
            }
        }

        private class TableHeader : Panel
        {
            private readonly Label _playerAliveCountLabel;
            private Label _karmaLabel;
            private Label _scoreLabel;
            private Label _pingLabel;

            public TableHeader(Panel parent)
            {
                Parent = parent;

                _playerAliveCountLabel = Add.Label("? players left", "name");
                _karmaLabel = Add.Label("Karma", "karma");
                _scoreLabel = Add.Label("Score", "score");
                _pingLabel = Add.Label("Ping", "ping");
            }

            public override void Tick()
            {
                _playerAliveCountLabel.Text = $"{Client.All.Count - TTTReborn.Gamemode.Game.GetConfirmedPlayers().Count} players left";
            }
        }

        private class ScoreboardGroup : Panel
        {
            public readonly string GroupTitle;
            public int GroupMembers = 0;
            private readonly Panel _groupContent;
            private readonly Panel _groupTitleWrapper;
            private readonly Label _groupTitleLabel;

            public ScoreboardGroup(Panel parent, string groupName)
            {
                Parent = parent;

                GroupTitle = groupName;

                AddClass(groupName);

                _groupTitleWrapper = Add.Panel("scoreboardGroup__title-wrapper");
                _groupTitleLabel = _groupTitleWrapper.Add.Label("", "scoreboardGroup__title");
                _groupContent = Add.Panel("scoreboardGroup__content");
            }

            // TODO: Implement logic for the player counter in the title
            public void UpdateLabel()
            {
                _groupTitleLabel.Text = $"{GroupTitle.ToUpper()}  -  {GroupMembers}";
            }

            public ScoreboardEntry AddEntry(PlayerScore.Entry entry)
            {
                ScoreboardEntry scoreboardEntry = _groupContent.AddChild<ScoreboardEntry>();
                scoreboardEntry.ScoreboardGroupName = GroupTitle;
                scoreboardEntry.SteamId = entry.Get<ulong>("steamid");

                scoreboardEntry.UpdateFrom(entry);

                return scoreboardEntry;
            }
        }

        private ScoreboardGroup AddScoreboardGroup(string groupName)
        {
            if (_scoreboardGroups.ContainsKey(groupName))
            {
                return _scoreboardGroups[groupName];
            }

            ScoreboardGroup scoreboardGroup = new ScoreboardGroup(_mainContent, groupName);
            scoreboardGroup.UpdateLabel();

            _scoreboardGroups.Add(groupName, scoreboardGroup);

            return scoreboardGroup;
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

        private ScoreboardGroup GetScoreboardGroup(PlayerScore.Entry entry)
        {
            string group = "Alive";

            if (!entry.Get<bool>("alive", true))
            {
                // TODO better spectator check, maybe with a player var
                group = "Dead";
            }
            else
            {
                ulong steamId = entry.Get<ulong>("steamid", 0);

                if (steamId != 0)
                {
                    foreach (Client client in Client.All)
                    {
                        if (client.Pawn.IsValid() && client.SteamId == steamId)
                        {
                            if ((client.Pawn as TTTPlayer).IsMissingInAction)
                            {
                                group = "MIA";
                            }

                            break;
                        }
                    }
                }
            }

            _scoreboardGroups.TryGetValue(group, out ScoreboardGroup scoreboardGroup);

            if (scoreboardGroup == null)
            {
                scoreboardGroup = AddScoreboardGroup(group);
            }

            return scoreboardGroup;
        }

        private void UpdatePlayer(PlayerScore.Entry entry)
        {
            if (_entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
            {
                ScoreboardGroup scoreboardGroup = GetScoreboardGroup(entry);

                if (scoreboardGroup.GroupTitle != panel.ScoreboardGroupName)
                {
                    // instead of remove and add, move the panel into the right parent
                    RemovePlayer(entry);
                    AddPlayer(entry);

                    DeleteEmptyScoreboardGroups();

                    return;
                }

                panel.UpdateFrom(entry);
            }
            else
            {
                // Add to queue? Up to now, just print an error #hacky
                Log.Error($"Tried to update the ScoreboardPanel of the player with sid: '{entry.Get<ulong>("steamid")}'");
            }
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

        private void DeleteEmptyScoreboardGroups()
        {
            List<string> removeList = new();

            foreach ((string key, ScoreboardGroup value) in _scoreboardGroups)
            {
                if (value.GroupMembers == 0)
                {
                    removeList.Add(key);
                }
            }

            foreach (string key in removeList)
            {
                _scoreboardGroups[key].Delete();

                _scoreboardGroups.Remove(key);
            }
        }

        private void RemovePlayer(PlayerScore.Entry entry)
        {
            if (_entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
            {
                _scoreboardGroups.TryGetValue(panel.ScoreboardGroupName, out ScoreboardGroup scoreboardGroup);

                if (scoreboardGroup != null)
                {
                    scoreboardGroup.GroupMembers--;
                }

                scoreboardGroup.UpdateLabel();

                panel.Delete();
                _entries.Remove(entry.Id);
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (Input.Down(InputButton.Score))
            {
                Log.Info("Hello");
            }

            SetClass("open", Input.Down(InputButton.Score));
        }
    }
}
