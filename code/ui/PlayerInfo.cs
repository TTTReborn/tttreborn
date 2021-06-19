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

        public class RolePanel : Panel
        {
            public Label RoleLabel { set; get; }

            private TTTRole currentRole;

            public RolePanel(Panel parent)
            {
                Parent = parent;

                RoleLabel = Add.Label("Unknown role", "rolelabel");
            }

            public override void Tick()
            {
                TTTPlayer player = Local.Pawn as TTTPlayer;

                if (player == null)
                {
                    return;
                }

                if (currentRole != player.Role)
                {
                    currentRole = player.Role;

                    Style.BackgroundColor = player.Role.Color;
                    Style.Dirty();

                    RoleLabel.Text = $"{player.Role.Name.ToUpper()}";

                    SetClass("hide", player.Role is NoneRole);
                }
            }
        }

        public class IndicatorsPanel : Panel
        {
            public BarPanel HealthBar { set; get; }
            public BarPanel AmmoBar { set; get; }

            // TODO rework event based
            private float currentHealth;
            private int currentAmmo;

            public IndicatorsPanel(Panel parent)
            {
                Parent = parent;

                HealthBar = new BarPanel(this, "", "healthlabel");
                HealthBar.AddClass("health");

                AmmoBar = new BarPanel(this, "", "ammolabel");
                AmmoBar.AddClass("ammo");
            }

            public override void Tick()
            {
                TTTPlayer player = Local.Pawn as TTTPlayer;

                if (player == null)
                {
                    return;
                }

                if (currentHealth != player.Health)
                {
                    currentHealth = player.Health;

                    HealthBar.TextLabel.Text = $"{player.Health:n0}";

                    HealthBar.Style.Width = Length.Percent(player.Health);
                    HealthBar.Style.Dirty();
                }

                TTTWeapon weapon = player.ActiveChild as TTTWeapon;
                bool isWeaponNull = weapon == null;

                AmmoBar.SetClass("hide", isWeaponNull || weapon.WeaponType == WeaponType.Melee);
                if (!isWeaponNull)
                {
                    if (currentAmmo == weapon.AmmoClip)
                    {
                        return;
                    }

                    currentAmmo = weapon.AmmoClip;

                    AmmoBar.TextLabel.Text = $"{weapon.AmmoClip} / {weapon.ClipSize}";

                    AmmoBar.Style.Width = Length.Percent(weapon.AmmoClip / (float) weapon.ClipSize * 100f);
                    AmmoBar.Style.Dirty();
                }
            }
        }
    }

    public class BarPanel : Panel
    {
        public Label TextLabel { set; get; }

        public BarPanel(Panel parent, string text, string name)
        {
            Parent = parent;

            TextLabel = Add.Label(text, name);
        }
    }
}
