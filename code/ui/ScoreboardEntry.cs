using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class ScoreboardEntry : Panel
    {
        private PlayerScore.Entry _entry;
        public string ScoreboardGroupName;
        public ulong SteamId;

        private readonly Label _roleColorLabel;
        private readonly Label _playerName;
        private readonly Label _karma;
        private readonly Label _score;
        private readonly Label _ping;

        private Client _client;
        private TTTRole _currentRole;

        public ScoreboardEntry()
        {
            AddClass("entry");

            _roleColorLabel = Add.Label("", "rolecolor");
            _playerName = Add.Label("PlayerName", "name");
            _karma = Add.Label("", "karma");
            _score = Add.Label("", "score");
            _ping = Add.Label("", "ping");
        }

        public virtual void UpdateFrom(PlayerScore.Entry entry)
        {
            _entry = entry;

            _playerName.Text = entry.GetString("name");
            _karma.Text = entry.Get<int>("karma", 0).ToString();
            _score.Text = entry.Get<int>("score", 0).ToString();
            _ping.Text = entry.Get<int>("ping", 0).ToString();

            // TOOD: Make this work on creation of this Entry
            // TODO add sin-based fading
            SetClass("me", Local.Client != null && _entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
        }

        public override void Tick()
        {
            if (_client == null)
            {
                foreach (Client loopClient in Client.All)
                {
                    if (loopClient.SteamId == SteamId)
                    {
                        _client = loopClient;

                        break;
                    }
                }
            }

            if (_client?.Pawn is not TTTPlayer player || _currentRole == player.Role)
            {
                return;
            }

            _currentRole = player.Role;

            _roleColorLabel.Style.BackgroundColor = player.Role is Roles.NoneRole ? player.Role.Color : player.Role.Color.WithAlpha(0.75f);
            _roleColorLabel.Style.Dirty();
        }
    }
}
