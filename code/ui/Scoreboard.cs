using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace TTTReborn.UI
{
    public class Scoreboard : Panel
    {
        public Scoreboard()
        {
            StyleSheet.Load("/ui/Scoreboard.scss");
            header = new Header();
            AddHeader();
            AddTableHeader();
            Panel mainContent = Add.Panel("mainContent");
            // TODO: create playergroups & for each playergroup:
            AddScoreboardGroup(mainContent);

            PlayerScore.OnPlayerAdded += AddPlayer;
            PlayerScore.OnPlayerRemoved += RemovePlayer;
            Add.Panel("footer");
            // TODO: Implement UpdatePlayer method
            // PlayerScore.OnPlayerUpdated += UpdatePlayer;

            // why is: var (item, index) ; throwing an error here?
            foreach (PlayerScore.Entry player in PlayerScore.All)
            {
                AddPlayer(player);
            }
        }

        public class Header : Panel
        {
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
            public Panel ScoreboardLogo;
            public Panel InformationHolder;
            public Label ServerName;
            public Label ServerInfo;
            public Label ServerDescription;
            public Panel Canvas;
        }
        public class ScoreboardGroup : Panel
        {
            // TODO: Implement logic for the player counter in the title
            public void UpdateLabel()
            {
                this.GroupTitleLabel.Text = this.GroupTitle.ToUpper() + " - " + this.GroupMember.ToString();
            }
            public int GroupMember;
            public string GroupTitle;
            public Label GroupTitleLabel;
            public Panel GroupContent;
            public Panel Canvas;


        }

        public Dictionary<int, ScoreboardEntry> Entries = new();

        public Dictionary<string, ScoreboardGroup> ScoreboardGroups = new();
        public Header header;

        protected void AddHeader()
        {
            header.Canvas = Add.Panel("header");
            header.ScoreboardLogo = header.Canvas.Add.Panel("scoreboardLogo");
            header.InformationHolder = header.Canvas.Add.Panel("informationHolder");
            header.ServerName = header.InformationHolder.Add.Label("Here will be the servername", "serverName");
            header.ServerInfo = header.InformationHolder.Add.Label(header.GetServerInfoStr(), "serverInfo");
            header.ServerDescription = header.InformationHolder.Add.Label("This is the server description: Lorem ipsum dolor sit  elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat", "serverDescription");
        }

        protected void AddTableHeader()
        {
            Panel tableHeader = Add.Panel("tableHeader");
            tableHeader.Add.Label("3 Innocents left", "name");
            tableHeader.Add.Label("Karma", "karma");
            tableHeader.Add.Label("Score", "score");
            tableHeader.Add.Label("Ping", "ping");
        }

        protected void AddScoreboardGroup(Panel Content)
        {
            // TODO: Set proper groups dynamicly
            string group = "alive";
            ScoreboardGroup scoreboardGroup = new ScoreboardGroup
            {

            };
            scoreboardGroup.GroupTitle = group;
            scoreboardGroup.GroupMember = 0;
            scoreboardGroup.Canvas = Content.Add.Panel("scoreboardGroup " + group);
            scoreboardGroup.GroupTitleLabel = scoreboardGroup.Canvas.Add.Label("", "scoreboardGroup__title");
            scoreboardGroup.UpdateLabel();
            scoreboardGroup.GroupContent = scoreboardGroup.Canvas.Add.Panel("scoreboardGroup__content");

            ScoreboardGroups[group] = scoreboardGroup;
        }

        protected void AddPlayer(PlayerScore.Entry entry)
        {
            // TODO: Get proper scoreboardGroup for entry
            string group = "alive";
            ScoreboardGroup scoreboardGroup = ScoreboardGroups[group];
            scoreboardGroup.GroupMember++;
            ScoreboardEntry p = scoreboardGroup.GroupContent.AddChild<ScoreboardEntry>();
            bool isOdd = (PlayerScore.All.Length % 2) != 0;
            p.UpdateFrom(entry, isOdd);
            scoreboardGroup.UpdateLabel();
            header.UpdateServerInfo();
            Entries[entry.Id] = p;
        }
        protected void RemovePlayer(PlayerScore.Entry entry)
        {
            if (Entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
            {
                string group = "alive";
                ScoreboardGroup scoreboardGroup = ScoreboardGroups[group];
                scoreboardGroup.GroupMember--;
                scoreboardGroup.UpdateLabel();
                panel.Delete();
                Entries.Remove(entry.Id);
            }
        }
        public override void Tick()
        {
            base.Tick();

            SetClass("open", Local.Client?.Input.Down(InputButton.Score) ?? false);
        }
    }

    public class ScoreboardEntry : Panel
    {
        public PlayerScore.Entry Entry;

        public Label PlayerName;
        public Label Karma;
        public Label Score;
        public Label Ping;

        public ScoreboardEntry()
        {
            AddClass("entry");

            PlayerName = Add.Label("PlayerName", "name");
            Karma = Add.Label("", "karma");
            Score = Add.Label("", "score");
            Ping = Add.Label("", "ping");
        }

        public virtual void UpdateFrom(PlayerScore.Entry entry, bool isOdd)
        {
            Entry = entry;

            PlayerName.Text = entry.GetString("name");
            Karma.Text = entry.Get<int>("karma", 0).ToString();
            Score.Text = entry.Get<int>("score", 0).ToString();
            Ping.Text = entry.Get<int>("ping", 0).ToString();

            SetClass("me", Local.Client != null && entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
            SetClass(isOdd ? "odd" : "even", true);
        }
    }
}
