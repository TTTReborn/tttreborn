using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class ScoreboardEntry : Panel
    {
        public PlayerScore.Entry Entry;
        public string ScoreboardGroupName;
        public Label RoleColorLabel;
        public Label PlayerName;
        public Label Karma;
        public Label Score;
        public Label Ping;
        public ulong SteamId;

        private Client client;
        private BaseRole currentRole;

        public ScoreboardEntry()
        {
            AddClass("entry");

            RoleColorLabel = Add.Label("", "rolecolor");
            PlayerName = Add.Label("PlayerName", "name");
            Karma = Add.Label("", "karma");
            Score = Add.Label("", "score");
            Ping = Add.Label("", "ping");
        }

        public virtual void UpdateFrom(PlayerScore.Entry entry)
        {
            Entry = entry;

            PlayerName.Text = entry.GetString("name");
            Karma.Text = entry.Get<int>("karma", 0).ToString();
            Score.Text = entry.Get<int>("score", 0).ToString();
            Ping.Text = entry.Get<int>("ping", 0).ToString();

            // TOOD: Make this work on creation of this Entry
            // TODO add sin-based fading
            SetClass("me", Local.Client != null && Entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
        }

        public override void Tick()
        {
            if (client == null)
            {
                foreach (Client loopClient in Client.All)
                {
                    if (loopClient.SteamId == SteamId)
                    {
                        client = loopClient;

                        break;
                    }
                }
            }

            if (client == null || client.Pawn == null || !(client.Pawn is TTTPlayer player) || currentRole == player.Role)
            {
                return;
            }

            currentRole = player.Role;

            RoleColorLabel.Style.BackgroundColor = player.Role.Color;
            RoleColorLabel.Style.Dirty();
        }
    }
}
