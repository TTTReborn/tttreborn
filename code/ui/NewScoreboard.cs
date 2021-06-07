using System.ComponentModel;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace TTTReborn.UI
{
    public class Scoreboard : Panel
    {
        public class Header : Panel
        {
            public Panel ScoreboardLogo;
            public Panel InformationHolder;
            public Label ServerName;
            public Label ServerInfo;
            public Label ServerDescription;
            public Panel Canvas;
        }
        public class ScoreboardGroup : Panel
        {
            public Label GroupTitle;
            public Panel GroupContent;
            public Panel Canvas;


        }

        // public TextEntry Input { get; protected set; }
        // public Dictionary<int, ScoreboardEntry> Entries = new();
        // public Dictionary<int, TeamSection> TeamSections = new();

        // public Panel ScoreboardHeader;
        // public Label ScoreboardTitle;


        public Scoreboard()
        {
            StyleSheet.Load("/ui/NewScoreboard.scss");
            AddHeader();
            AddTableHeader();

            var mainContent = Add.Panel("mainContent");
            // For each playergroup:
            AddScoreboardGroup(mainContent);

            // PlayerScore.OnPlayerAdded += AddPlayer;
            // PlayerScore.OnPlayerUpdated += UpdatePlayer;
            // PlayerScore.OnPlayerRemoved += RemovePlayer;

            // foreach ( var player in PlayerScore.All )
            // {
            // 	AddPlayer( player );
            // }
        }

        protected void AddHeader()
        {
            var header = new Header
            {

            };


            // Set up the Container for the Team on the scoreboard
            header.Canvas = Add.Panel("header");
            header.ScoreboardLogo = header.Canvas.Add.Panel("scoreboardLogo");
            header.InformationHolder = header.Canvas.Add.Panel("informationHolder");
            header.ServerName = header.InformationHolder.Add.Label("Here will be the servername");
            header.ServerInfo = header.InformationHolder.Add.Label("10/32 - ttt_minecraft_b5 - 3 rounds / 09:41 left", "serverInfo");
            header.ServerDescription = header.InformationHolder.Add.Label("This is the server description: Lorem ipsum dolor sit  elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat", "serverDescription");

        }

        protected void AddTableHeader()
        {
            var tableHeader = Add.Panel("tableHeader");
            tableHeader.Add.Label("3 Innocents left", "name");
            tableHeader.Add.Label("Karma", "karma");
            tableHeader.Add.Label("Score", "score");
            tableHeader.Add.Label("Ping", "ping");
        }

        protected void AddScoreboardGroup(Panel Content)
        {
            var scoreboardGroup = new ScoreboardGroup
            {

            };

            scoreboardGroup.Canvas = Content.Add.Panel("scoreboardGroup alive");
            scoreboardGroup.GroupTitle = scoreboardGroup.Canvas.Add.Label("ALIVE - 8", "scoreboardGroup__title");
            scoreboardGroup.GroupContent = scoreboardGroup.Canvas.Add.Panel("scoreboardGroup__content");
            int index = 0;
            // why is: var (item, index) ; throwing an error here?
            foreach (var player in PlayerScore.All)
            {
                var p = scoreboardGroup.GroupContent.AddChild<ScoreboardEntry>();
                bool isOdd = (index % 2) != 0;
                index++;
                p.UpdateFrom(player, isOdd);
                p = scoreboardGroup.GroupContent.AddChild<ScoreboardEntry>();
                isOdd = (index % 2) != 0;
                index++;
                p.UpdateFrom(player, isOdd);
                p = scoreboardGroup.GroupContent.AddChild<ScoreboardEntry>();
                isOdd = (index % 2) != 0;
                index++;
                p.UpdateFrom(player, isOdd);
                p = scoreboardGroup.GroupContent.AddChild<ScoreboardEntry>();
                isOdd = (index % 2) != 0;
                index++;
                p.UpdateFrom(player, isOdd);
                p = scoreboardGroup.GroupContent.AddChild<ScoreboardEntry>();
                isOdd = (index % 2) != 0;
                index++;
                p.UpdateFrom(player, isOdd);
                p = scoreboardGroup.GroupContent.AddChild<ScoreboardEntry>();
                isOdd = (index % 2) != 0;
                index++;
                p.UpdateFrom(player, isOdd);
            }

            // tableHeader.Add.Label("3 Innocents left", "groupTitle");
        }

        protected void AddPlayer(PlayerScore.Entry entry) {
            // Do sort to propper scoreboardGroup here
            //  scoreboardGroup
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
			AddClass( "entry" );

			PlayerName = Add.Label( "PlayerName", "name" );
			Karma = Add.Label( "", "karma" );
			Score = Add.Label( "", "score" );
			Ping = Add.Label( "", "ping" );
		}

		public virtual void UpdateFrom( PlayerScore.Entry entry, bool isOdd )
		{
			Entry = entry;

			PlayerName.Text = entry.GetString( "name" );
			Karma.Text = entry.Get<int>( "karma", 0 ).ToString();
			Score.Text = entry.Get<int>( "score", 0 ).ToString();
			Ping.Text = entry.Get<int>( "ping", 0 ).ToString();

			SetClass( "me", Local.Client != null && entry.Get<ulong>( "steamid", 0 ) == Local.Client.SteamId );
            SetClass(isOdd ? "odd" : "even" , true);
        }
	}
	}
