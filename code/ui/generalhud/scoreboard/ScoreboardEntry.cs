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

        public ScoreboardEntry()
        {
            AddClass("entry");

            _roleColorLabel = Add.Label("", "rolecolor");
            _playerName = Add.Label("PlayerName", "name");
            _karma = Add.Label("", "karma");
            _score = Add.Label("", "score");
            _ping = Add.Label("", "ping");

            Initialize();
        }

        public virtual void UpdateFrom(PlayerScore.Entry entry)
        {
            _entry = entry;

            _playerName.Text = entry.GetString("name");
            _karma.Text = entry.Get<int>("karma", 0).ToString();
            _score.Text = entry.Get<int>("score", 0).ToString();
            _ping.Text = entry.Get<int>("ping", 0).ToString();

            if (_client == null)
            {
                Initialize();
            }

            if (_client?.Pawn is not TTTPlayer player)
            {
                return;
            }

            _roleColorLabel.Style.BackgroundColor = player.Role is NoneRole ? player.Role.Color : player.Role.Color.WithAlpha(0.75f);
            _roleColorLabel.Style.Dirty();
        }

        private void Initialize()
        {
            foreach (Client loopClient in Client.All)
            {
                if (loopClient.SteamId == SteamId)
                {
                    _client = loopClient;

                    break;
                }
            }

            SetClass("me", SteamId == Local.Client?.SteamId);
        }
    }
}
