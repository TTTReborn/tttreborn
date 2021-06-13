using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class Scoreboard : Panel
    {
        private Dictionary<int, ScoreboardEntry> Entries = new();

        private Dictionary<string, ScoreboardGroup> ScoreboardGroups = new();

        private Header header;

        private TableHeader tableHeader;

        private Panel mainContent;

        private Panel footer;

        public Scoreboard()
        {
            StyleSheet.Load("/ui/Scoreboard.scss");

            header = new Header(this);
            tableHeader = new TableHeader(this);

            mainContent = Add.Panel("mainContent");

            // TODO: create playergroups & for each playergroup:
            AddScoreboardGroup("Alive");

            PlayerScore.OnPlayerAdded += AddPlayer;
            PlayerScore.OnPlayerUpdated += UpdatePlayer;
            PlayerScore.OnPlayerRemoved += RemovePlayer;

            footer = Add.Panel("footer");

            // TODO: Implement UpdatePlayer method
            // PlayerScore.OnPlayerUpdated += UpdatePlayer;

            foreach (PlayerScore.Entry player in PlayerScore.All)
            {
                AddPlayer(player);
            }
        }

        public class Header : Panel
        {
            public Panel ScoreboardLogo;
            public Panel InformationHolder;
            public Label ServerName;
            public Label ServerInfo;
            public Label ServerDescription;

            public Header(Panel parent)
            {
                Parent = parent;

                ScoreboardLogo = Add.Panel("scoreboardLogo");
                InformationHolder = Add.Panel("informationHolder");
                ServerName = InformationHolder.Add.Label("Here will be the servername", "serverName");
                ServerInfo = InformationHolder.Add.Label(GetServerInfoStr(), "serverInfo");
                ServerDescription = InformationHolder.Add.Label("This is the server description: Lorem ipsum dolor sit  elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat", "serverDescription");
            }

            public void UpdateServerInfo()
            {
                this.ServerInfo.Text = this.GetServerInfoStr();
            }

            public string GetServerInfoStr()
            {
                // TODO: Get this out of the header
                // TODO: Fill the other variables
                return $"{PlayerScore.All.Length}/MAXPLAYER - MAPNAME - ROUND/TIME LEFT";
            }
        }

        public class TableHeader : Panel
        {
            public Label PlayerAliveCountLabel;
            public Label KarmaLabel;
            public Label ScoreLabel;
            public Label PingLabel;

            public TableHeader(Panel parent)
            {
                Parent = parent;

                PlayerAliveCountLabel = Add.Label("? Innocents left", "name");
                KarmaLabel = Add.Label("Karma", "karma");
                ScoreLabel = Add.Label("Score", "score");
                PingLabel = Add.Label("Ping", "ping");
            }
        }

        public class ScoreboardGroup : Panel
        {
            public string GroupTitle { get; private set; }
            public Panel GroupContent;
            private Label groupTitleLabel;

            public ScoreboardGroup(Panel parent, string groupName)
            {
                Parent = parent;

                GroupTitle = groupName;

                AddClass(groupName);

                groupTitleLabel = Add.Label("", "scoreboardGroup__title");
                GroupContent = Add.Panel("scoreboardGroup__content");
            }

            // TODO: Implement logic for the player counter in the title
            public void UpdateLabel()
            {
                groupTitleLabel.Text = GroupTitle.ToUpper();
            }

            public ScoreboardEntry AddEntry(PlayerScore.Entry entry)
            {
                ScoreboardEntry scoreboardEntry = GroupContent.AddChild<ScoreboardEntry>();

                scoreboardEntry.UpdateFrom(entry, PlayerScore.All.Length % 2 != 0);

                return scoreboardEntry;
            }
        }

        private void AddScoreboardGroup(string groupName)
        {
            if (ScoreboardGroups.ContainsKey(groupName))
            {
                return;
            }

            ScoreboardGroup scoreboardGroup = new ScoreboardGroup(mainContent, groupName);
            scoreboardGroup.UpdateLabel();

            ScoreboardGroups.Add(groupName, scoreboardGroup);
        }

        private void AddPlayer(PlayerScore.Entry entry)
        {
            // TODO: Get proper scoreboardGroup for entry
            //bool alive = entry.Get<bool>("alive");

            if (ScoreboardGroups.TryGetValue("Alive", out ScoreboardGroup scoreboardGroup))
            {
                Entries.Add(entry.Id, scoreboardGroup.AddEntry(entry));

                scoreboardGroup.UpdateLabel();
                header.UpdateServerInfo();
            }
        }

        private void UpdatePlayer(PlayerScore.Entry entry)
        {
            if (Entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
            {
                panel.UpdateFrom(entry, entry.Id % 2 == 0);

                // TODO remove label / group if empty
            }
        }

        private void RemovePlayer(PlayerScore.Entry entry)
        {
            if (Entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
            {
                if (ScoreboardGroups.TryGetValue("Alive", out ScoreboardGroup scoreboardGroup))
                {
                    scoreboardGroup.UpdateLabel();
                }

                panel.Delete();
                Entries.Remove(entry.Id);
            }
        }

        public override void Tick()
        {
            base.Tick();

            SetClass("open", Input.Down(InputButton.Score));
        }
    }
}
