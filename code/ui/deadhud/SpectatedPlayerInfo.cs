using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class SpectatedPlayerInfo : Panel
    {
        public SpectatedPlayerInfo()
        {
            StyleSheet.Load("/ui/deadhud/SpectatedPlayerInfo.scss");

            new NamePanel(this);
            new IndicatorsPanel(this);
        }

        private class NamePanel : Panel
        {
            private readonly Label _roleLabel;

            private TTTRole _currentRole;

            public NamePanel(Panel parent)
            {
                Parent = parent;

                _roleLabel = Add.Label("Unknown player", "namelabel");
            }

            public override void Tick()
            {
                base.Tick();

                if (Local.Pawn is not TTTPlayer player)
                {
                    return;
                }

                // if (_currentRole != player.Role)
                // {
                //     _currentRole = player.Role;

                //     Style.BackgroundColor = player.Role.Color;
                //     Style.Dirty();

                //     _roleLabel.Text = $"{player.Role.Name.ToUpper()}";

                //     SetClass("hide", player.Role is NoneRole);
                // }
            }
        }

        private class IndicatorsPanel : Panel
        {
            private readonly BarPanel _healthBar;

            // TODO rework event based
            private float _currentHealth = -1;

            public IndicatorsPanel(Panel parent)
            {
                Parent = parent;

                _healthBar = new BarPanel(this, "", "healthbar");
                _healthBar.AddClass("health");
            }

            public override void Tick()
            {
                base.Tick();

                TTTPlayer player = Local.Pawn as TTTPlayer;

                if (player == null)
                {
                    return;
                }

                // if (_currentHealth != player.Health)
                // {
                //     _currentHealth = player.Health;

                //     _healthBar.TextLabel.Text = $"{player.Health:n0}";

                //     _healthBar.Style.Width = Length.Percent(player.Health / player.MaxHealth * 100f);
                //     _healthBar.Style.Dirty();
                // }
            }
        }
    }
}
