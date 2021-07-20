using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class PlayerInfo : ObservablePanel
    {
        public PlayerInfo() : base()
        {
            StyleSheet.Load("/ui/generalhud/playerinfo/PlayerInfo.scss");

            new RolePanel(this);
            new IndicatorsPanel(this);

            AutoHideWithoutObservablePlayer = true;
        }

        private class RolePanel : ObservablePanel
        {
            private readonly Label _roleLabel;

            private TTTRole _currentRole;

            public RolePanel(Panel parent) : base()
            {
                Parent = parent;

                _roleLabel = Add.Label("Unknown role", "rolelabel");
            }

            public override void Tick()
            {
                base.Tick();

                if (ObservedPlayer == null)
                {
                    return;
                }

                if (_currentRole != ObservedPlayer.Role)
                {
                    _currentRole = ObservedPlayer.Role;

                    if (ObservedPlayer.Role is NoneRole)
                    {
                        Style.BackgroundColor = Color.Black;
                    }
                    else
                    {
                        Style.BackgroundColor = ObservedPlayer.Role.Color;
                    }

                    Style.Dirty();

                    if (IsObserving)
                    {
                        _roleLabel.Text = $"{ObservedPlayer.GetClientOwner()?.Name}";

                        SetClass("hide", false);
                    }
                    else
                    {
                        _roleLabel.Text = $"{ObservedPlayer.Role.Name.ToUpper()}";

                        SetClass("hide", ObservedPlayer.Role is NoneRole);
                    }
                }
            }
        }

        private class IndicatorsPanel : ObservablePanel
        {
            private readonly BarPanel _healthBar;
            private readonly BarPanel _staminaBar;

            // TODO rework event based
            private float _currentHealth = -1;
            private float _currentStamina = -1;

            public IndicatorsPanel(Panel parent) : base()
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

                if (ObservedPlayer == null)
                {
                    return;
                }

                if (_currentHealth != ObservedPlayer.Health)
                {
                    _currentHealth = ObservedPlayer.Health;

                    _healthBar.TextLabel.Text = $"{ObservedPlayer.Health:n0}";

                    _healthBar.Style.Width = Length.Percent(ObservedPlayer.Health / ObservedPlayer.MaxHealth * 100f);
                    _healthBar.Style.Dirty();
                }

                if (ObservedPlayer.Controller is DefaultWalkController && DefaultWalkController.IsSprintEnabled)
                {
                    _staminaBar.Style.Display = DisplayMode.Flex;

                    _staminaBar.SetClass("hide", ObservedPlayer.LifeState != LifeState.Alive);

                    if (_currentStamina == ObservedPlayer.Stamina)
                    {
                        return;
                    }

                    _currentStamina = ObservedPlayer.Stamina;

                    _staminaBar.TextLabel.Text = $"{ObservedPlayer.Stamina:n0}";

                    _staminaBar.Style.Width = Length.Percent(ObservedPlayer.Stamina / ObservedPlayer.MaxStamina * 100f);
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
