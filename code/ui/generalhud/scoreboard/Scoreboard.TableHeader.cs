using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;

namespace TTTReborn.UI
{
    public partial class Scoreboard
    {
        private class TableHeader : Panel
        {
            private readonly Label _playerAliveCountLabel;
            private readonly Label _karmaLabel;
            private readonly Label _scoreLabel;
            private readonly Label _pingLabel;

            public TableHeader(Panel parent)
            {
                Parent = parent;

                _playerAliveCountLabel = Add.Label("? players left", "name");
                _karmaLabel = Add.Label("Karma", "karma");
                _scoreLabel = Add.Label("Score", "score");
                _pingLabel = Add.Label("Ping", "ping");
            }

            public override void Tick()
            {
                _playerAliveCountLabel.Text = $"{Client.All.Count - Utils.GetConfirmedPlayers().Count} players left";
            }
        }
    }
}
