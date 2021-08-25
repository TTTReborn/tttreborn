using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;
using TTTReborn.Settings;

namespace TTTReborn.UI
{
    public class PlayerInfo : Panel
    {
        public PlayerInfo()
        {
            StyleSheet.Load("/ui/generalhud/playerinfo/PlayerInfo.scss");

            new RolePanel(this);
            new IndicatorsPanel(this);
        }

        public override void Tick()
        {
            base.Tick();

            IsShowing = Local.Pawn is TTTPlayer player && (!player.IsSpectator && !player.IsSpectatingPlayer);
        }

        private class RolePanel : Panel
        {
            private readonly TranslationLabel _roleLabel;

            private TTTRole _currentRole;
            private TTTPlayer _currentPlayer;

            public RolePanel(Sandbox.UI.Panel parent)
            {
                Parent = parent;

                _roleLabel = Add.TranslationLabel("", "rolelabel");
            }

            public override void Tick()
            {
                base.Tick();

                if (Local.Pawn is not TTTPlayer player || _currentPlayer == player.CurrentPlayer && _currentRole == player.CurrentPlayer.Role)
                {
                    return;
                }

                _currentPlayer = player.CurrentPlayer;
                _currentRole = _currentPlayer.Role;

                Style.BackgroundColor = _currentPlayer.Role is NoneRole
                    ? Color.Black
                    : _currentPlayer.Role.Color;

                Style.Dirty();

                if (player.IsSpectatingPlayer)
                {
                    _roleLabel.SetTranslation(null); // disable auto-update Text on language change
                    _roleLabel.Text = $"{_currentPlayer.GetClientOwner()?.Name}";

                    IsShowing = true;
                }
                else
                {
                    _roleLabel.SetTranslation(_currentRole.GetRoleTranslationKey("ROLE_NAME"));

                    IsShowing = !(player.Role is NoneRole);
                }
            }
        }

        private class IndicatorsPanel : Panel
        {
            private readonly BarPanel _healthBar;
            private readonly BarPanel _staminaBar;

            // TODO rework event based
            private float _currentHealth = -1;
            private float _currentStamina = -1;

            public IndicatorsPanel(Sandbox.UI.Panel parent)
            {
                Parent = parent;

                _healthBar = new BarPanel(this, "", "healthbar");
                _healthBar.AddClass("health");

                _staminaBar = new BarPanel(this, "", "staminabar");
                _staminaBar.AddClass("stamina");
            }

            public override void Tick()
            {
                base.Tick();

                if (Local.Pawn is not TTTPlayer player)
                {
                    return;
                }

                if (_currentHealth != player.CurrentPlayer.Health)
                {
                    _currentHealth = player.CurrentPlayer.Health;

                    _healthBar.TextLabel.Text = $"{player.CurrentPlayer.Health:n0}";

                    _healthBar.IsShowing = (player.CurrentPlayer.LifeState == LifeState.Alive);

                    _healthBar.Style.Width = Length.Percent(player.CurrentPlayer.Health / player.CurrentPlayer.MaxHealth * 100f);
                    _healthBar.Style.Dirty();
                }

                if (player.CurrentPlayer.Controller is DefaultWalkController && DefaultWalkController.IsSprintEnabled)
                {
                    _staminaBar.Style.Display = DisplayMode.Flex;

                    _staminaBar.IsShowing = !(player.CurrentPlayer.LifeState != LifeState.Alive);

                    if (_currentStamina == player.CurrentPlayer.Stamina)
                    {
                        _staminaBar.Style.Dirty();

                        return;
                    }

                    _currentStamina = player.CurrentPlayer.Stamina;

                    _staminaBar.TextLabel.Text = $"{player.CurrentPlayer.Stamina:n0}";

                    _staminaBar.Style.Width = Length.Percent(player.CurrentPlayer.Stamina / player.CurrentPlayer.MaxStamina * 100f);
                    _staminaBar.Style.Dirty();
                }
                else
                {
                    _staminaBar.Style.Display = DisplayMode.None;
                    _staminaBar.Style.Dirty();
                }
            }
        }
    }
}
