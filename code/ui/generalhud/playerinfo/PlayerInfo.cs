using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;
using TTTReborn.Roles;

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

        private class RolePanel : Panel
        {
            private readonly Label _roleLabel;

            private TTTRole _currentRole;

            public RolePanel(Panel parent)
            {
                Parent = parent;

                _roleLabel = Add.Label("Unknown role", "rolelabel");
            }

            public override void Tick()
            {
                base.Tick();

                if (Local.Pawn is not TTTPlayer player)
                {
                    return;
                }

                if (_currentRole == player.ObservingPlayer.Role)
                {
                    return;
                }

                _currentRole = player.ObservingPlayer.Role;

                Style.BackgroundColor = player.ObservingPlayer.Role is NoneRole
                    ? Color.Black
                    : player.ObservingPlayer.Role.Color;

                Style.Dirty();

                if (player.IsObservingPlayer)
                {
                    _roleLabel.Text = $"{player.ObservingPlayer.GetClientOwner()?.Name}";

                    SetClass("hide", false);
                }
                else
                {
                    _roleLabel.Text = $"{player.Role.Name.ToUpper()}";

                    SetClass("hide", player.Role is NoneRole);
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

            public IndicatorsPanel(Panel parent)
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

                if (_currentHealth != player.ObservingPlayer.Health)
                {
                    _currentHealth = player.ObservingPlayer.Health;

                    _healthBar.TextLabel.Text = $"{player.ObservingPlayer.Health:n0}";

                    _healthBar.Style.Width = Length.Percent(player.ObservingPlayer.Health / player.ObservingPlayer.MaxHealth * 100f);
                    _healthBar.Style.Dirty();
                }

                if (player.ObservingPlayer.Controller is DefaultWalkController && DefaultWalkController.IsSprintEnabled)
                {
                    _staminaBar.Style.Display = DisplayMode.Flex;

                    _staminaBar.SetClass("hide", player.ObservingPlayer.LifeState != LifeState.Alive);

                    if (_currentStamina == player.ObservingPlayer.Stamina)
                    {
                        return;
                    }

                    _currentStamina = player.ObservingPlayer.Stamina;

                    _staminaBar.TextLabel.Text = $"{player.ObservingPlayer.Stamina:n0}";

                    _staminaBar.Style.Width = Length.Percent(player.ObservingPlayer.Stamina / player.ObservingPlayer.MaxStamina * 100f);
                    _staminaBar.Style.Dirty();
                }
                else
                {
                    _staminaBar.Style.Display = DisplayMode.None;
                }
            }
        }
    }
}
