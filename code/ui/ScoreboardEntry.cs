using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
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
