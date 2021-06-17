using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class ScoreboardEntry : Panel
    {
        public PlayerScore.Entry Entry;
        public string ScoreboardGroupName;
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

        public virtual void UpdateFrom(PlayerScore.Entry entry)
        {
            Entry = entry;

            UpdateRoleClass(entry);

            PlayerName.Text = entry.GetString("name");
            Karma.Text = entry.Get<int>("karma", 0).ToString();
            Score.Text = entry.Get<int>("score", 0).ToString();
            Ping.Text = entry.Get<int>("ping", 0).ToString();

            // TOOD: Make this work on creation of this Entry
            SetClass("me", Local.Client != null && Entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
        }

        public void UpdateRoleClass(PlayerScore.Entry entry)
        {
            bool isMarkedAsTraitor = false;
            bool isEntryRoleTraitor = entry.Get<string>("role") == "Traitor";
            if (isEntryRoleTraitor)
            {
                bool isYourselfTraitor = Local.Client.Pawn is TTTPlayer player && player.Role is Roles.TraitorRole;
                bool isEntryDead = !entry.Get<bool>("alive");

                isMarkedAsTraitor = isYourselfTraitor || isEntryDead;
            }
            this.SetClass("traitor", isMarkedAsTraitor);
        }
    }
}
