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
            StyleSheet.Load("/ui/PlayerInfo.scss");

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

                if (_currentRole != player.Role)
                {
                    _currentRole = player.Role;

                    Style.BackgroundColor = player.Role.Color;
                    Style.Dirty();

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

                TTTPlayer player = Local.Pawn as TTTPlayer;

                if (player == null)
                {
                    return;
                }

                if (_currentHealth != player.Health)
                {
                    _currentHealth = player.Health;

                    _healthBar.TextLabel.Text = $"{player.Health:n0}";

                    _healthBar.Style.Width = Length.Percent(player.Health);
                    _healthBar.Style.Dirty();
                }

                if (player.Controller is DefaultWalkController && DefaultWalkController.IsSprintEnabled)
                {
                    _staminaBar.Style.Display = DisplayMode.Flex;

                    _staminaBar.SetClass("hide", player.LifeState != LifeState.Alive);

                    if (_currentStamina == player.Stamina)
                    {
                        return;
                    }

                    _currentStamina = player.Stamina;

                    _staminaBar.TextLabel.Text = $"{player.Stamina:n0}";

                    _staminaBar.Style.Width = Length.Percent(player.Stamina);
                    _staminaBar.Style.Dirty();
                }
                else
                {
                    _staminaBar.Style.Display = DisplayMode.None;
                }
            }
        }
    }

    public class BarPanel : Panel
    {
        public readonly Label TextLabel;

        public BarPanel(Panel parent, string text, string name)
        {
            Parent = parent;

            TextLabel = Add.Label(text, name);
        }
    }
}
