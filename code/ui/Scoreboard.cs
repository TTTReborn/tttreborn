using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
    {

        public Scoreboard()
        {
            StyleSheet.Load("/ui/Scoreboard.scss");

        }
        public class ScoreboardHeader : Panel
        {
            public Label ServerName { set; get; }
            public Label ServerDescription { set; get; }

        }
        protected override void AddHeader()
        {
            // Is the the table header?
            Header = Add.Panel("header");
            Header.Add.Label("Player", "name");
            Header.Add.Label("Karma", "karma");
            Header.Add.Label("Score", "score");
            Header.Add.Label("Deaths", "deaths");
            Header.Add.Label("Ping", "ping");
        }
    }

    public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
    {
        public class EntryPicture : Panel
        {
            public Label ServerName { set; get; }
            public Label ServerDescription {    set; get; }

        }
        public Label PlayerName;
        public Label Score;
        public Label Karma;
        public Label Ping;

        public ScoreboardEntry() : base()
        {
            AddClass("entry");

            PlayerName = Add.Label(entry.GetString("name"), "playername");
            Score = Add.Label("", "score");
            Karma = Add.Label("", "karma");
            Ping = Add.Label("", "ping");
        }

        public override void UpdateFrom(PlayerScore.Entry entry)
        {
            Entry = entry;

            PlayerName.Text = entry.GetString("name"); // Does the playername need to update? In orig. you get kicked if you change ur name
            Score.Text = entry.Get<int>("kills", 0).ToString();
            Deaths.Text = entry.Get<int>("deaths", 0).ToString();
            Karma.Text = entry.Get<int>("karma", 0).ToString();
            Ping.Text = entry.Get<int>("ping", 0).ToString();

            SetClass("me", Local.Client != null && entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
        }
    }

}
