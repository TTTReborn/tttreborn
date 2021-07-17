using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Player.Camera;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class SpectatedPlayerInfo : Panel
    {
        private TTTPlayer _currentPlayer;
        private NamePanel _namePanel;
        private IndicatorsPanel _indicatorsPanel;

        public SpectatedPlayerInfo()
        {
            StyleSheet.Load("/ui/deadhud/SpectatedPlayerInfo.scss");

            _namePanel = new NamePanel(this);
            _indicatorsPanel = new IndicatorsPanel(this);
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player || player.Camera is not ThirdPersonSpectateCamera spectateCamera)
            {
                SetClass("hide", true);

                return;
            }

            SetClass("hide", spectateCamera.TargetPlayer == null);

            if (_currentPlayer != spectateCamera.TargetPlayer)
            {
                _currentPlayer = spectateCamera.TargetPlayer;

                _namePanel.SetTargetPlayer(_currentPlayer);
                _indicatorsPanel.SetTargetPlayer(_currentPlayer);
            }
        }

        private class NamePanel : Panel
        {
            private TTTPlayer _currentPlayer;
            private readonly Label _nameLabel;

            public NamePanel(Panel parent)
            {
                Parent = parent;

                _nameLabel = Add.Label("Unknown player", "namelabel");
            }

            public void SetTargetPlayer(TTTPlayer player)
            {
                _currentPlayer = player;
            }

            public override void Tick()
            {
                base.Tick();

                if (_currentPlayer == null)
                {
                    return;
                }

                if (_currentPlayer.Role is NoneRole)
                {
                    Style.BackgroundColor = Color.Black;
                }
                else
                {
                    Style.BackgroundColor = _currentPlayer.Role.Color;
                }

                Style.Dirty();

                _nameLabel.Text = $"{_currentPlayer.GetClientOwner()?.Name}";
            }
        }

        private class IndicatorsPanel : Panel
        {
            private TTTPlayer _currentPlayer;
            private readonly BarPanel _healthBar;

            // TODO rework event based
            private float _currentHealth = -1;

            public IndicatorsPanel(Panel parent)
            {
                Parent = parent;

                _healthBar = new BarPanel(this, "", "healthbar");
                _healthBar.AddClass("health");
            }

            public void SetTargetPlayer(TTTPlayer player)
            {
                _currentPlayer = player;
            }

            public override void Tick()
            {
                base.Tick();

                if (_currentPlayer == null)
                {
                    return;
                }

                if (_currentHealth != _currentPlayer.Health)
                {
                    _currentHealth = _currentPlayer.Health;

                    _healthBar.TextLabel.Text = $"{_currentPlayer.Health:n0}";

                    _healthBar.Style.Width = Length.Percent(_currentPlayer.Health / _currentPlayer.MaxHealth * 100f);
                    _healthBar.Style.Dirty();
                }
            }
        }
    }
}
