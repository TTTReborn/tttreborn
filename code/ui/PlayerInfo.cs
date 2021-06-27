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
            private readonly BarPanel _ammoBar;

            // TODO rework event based
            private float _currentHealth = -1;
            private int _currentAmmo = -1;

            public IndicatorsPanel(Panel parent)
            {
                Parent = parent;

                _healthBar = new BarPanel(this, "", "healthlabel");
                _healthBar.AddClass("health");

                _ammoBar = new BarPanel(this, "", "ammolabel");
                _ammoBar.AddClass("ammo");
            }

            public override void Tick()
            {
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

                ICarriableItem carriable = player.ActiveChild as ICarriableItem;

                bool isWeaponNull = carriable == null || carriable is not TTTWeapon;

                _ammoBar.SetClass("hide", isWeaponNull || carriable.HoldType == HoldType.Melee);

                if (!isWeaponNull)
                {
                    TTTWeapon weapon = carriable as TTTWeapon;

                    if (_currentAmmo == weapon.AmmoClip)
                    {
                        return;
                    }

                    _currentAmmo = weapon.AmmoClip;

                    _ammoBar.TextLabel.Text = $"{weapon.AmmoClip} / {weapon.ClipSize}";

                    _ammoBar.Style.Width = Length.Percent(weapon.AmmoClip / (float) weapon.ClipSize * 100f);
                    _ammoBar.Style.Dirty();
                }
                else
                {
                    _currentAmmo = -1;
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
