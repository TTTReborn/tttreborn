using Sandbox;
using Sandbox.UI;

using TTTReborn.Roles;

#pragma warning disable IDE0052

namespace TTTReborn.UI
{
    [UseTemplate]
    public class ScoreboardEntry : Panel
    {
        public string ScoreboardGroupName;
        public IClient Client;

        private Image PlayerAvatar { get; set; }
        private string PlayerName { get; set; }

        private string Score { get; set; }
        private string Karma { get; set; }
        private string Ping { get; set; }

        private float _nextUpdate = 0f;

        public virtual void Update()
        {
            if (Client == null)
            {
                return;
            }

            PlayerName = Client.Name;
            Score = Client.GetInt("score").ToString();
            Karma = Client.GetInt("karma").ToString();

            SetClass("me", Client == Game.LocalClient);

            PlayerAvatar.SetTexture($"avatar:{Client.SteamId}");

            if (Client.Pawn is Player player && player.Role is not NoneRole && player.Role is not InnocentRole)
            {
                Style.BackgroundColor = player.Role.Color.WithAlpha(0.15f);
            }
            else
            {
                Style.BackgroundColor = null;
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (Client != null && _nextUpdate < Time.Now)
            {
                _nextUpdate = Time.Now + 1f;

                Ping = Client.Ping.ToString();
            }
        }
    }
}
