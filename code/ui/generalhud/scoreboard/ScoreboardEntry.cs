using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class ScoreboardEntry : Panel
    {
        public string ScoreboardGroupName;
        public Client Client;

        private Image _playerAvatar;
        private readonly Label _playerName;

        private readonly Label _karma;
        private readonly Label _ping;

        public ScoreboardEntry()
        {
            AddClass("text-shadow");
            AddClass("entry");

            _playerAvatar = Add.Image();
            _playerAvatar.AddClass("circular");
            _playerAvatar.AddClass("avatar");

            _playerName = Add.Label();
            _playerName.AddClass("name-label");

            _karma = Add.Label("", "karma");
            _ping = Add.Label("", "ping");
        }

        public virtual void Update()
        {
            if (Client == null)
            {
                return;
            }

            _playerName.Text = Client.Name;
            _karma.Text = Client.GetInt("karma").ToString();

            SetClass("me", Client == Local.Client);

            if (Client.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (player.Role is not NoneRole && player.Role is not InnocentRole)
            {
                Style.BackgroundColor = player.Role.Color.WithAlpha(0.15f);
            }
            else
            {
                Style.BackgroundColor = null;
            }

            Style.Dirty();

            _playerAvatar.SetTexture($"avatar:{Client.SteamId}");
        }

        public override void Tick()
        {
            base.Tick();

            if (Client != null)
            {
                _ping.Text = Client.Ping.ToString();
            }
        }
    }
}
