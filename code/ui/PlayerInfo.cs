using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Weapons;
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

            private BaseRole currentRole;

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
                    if (currentRole != null)
                    {
                        // Remove RolePanel .class for the old role:
                        SetClass(currentRole.Name, false);
                    }

                    currentRole = player.Role;

                    // Give RolePanel .class for the matching role:
                    SetClass(currentRole.Name, true);

                    RoleLabel.Text = $"{player.Role.Name}";
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

            private bool invisibleAmmo;

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

                    // bar width
                    HealthBar.Style.Width = Length.Percent(player.Health);
                    HealthBar.Style.Dirty();
                }

                var weapon = player.ActiveChild as Weapon;

                if (weapon != null)
                {
                    if (currentAmmo == weapon.AmmoClip)
                    {
                        return;
                    }

                    currentAmmo = weapon.AmmoClip;

                    if (invisibleAmmo)
                    {
                        invisibleAmmo = false;

                        AmmoBar.RemoveClass("invisible");
                    }

                    AmmoBar.TextLabel.Text = $"{weapon.AmmoClip}";

                    // bar width
                    AmmoBar.Style.Width = Length.Percent(weapon.AmmoClip / (float) weapon.ClipSize * 100f);
                    AmmoBar.Style.Dirty();
                }
                else if (!invisibleAmmo)
                {
                    invisibleAmmo = true;

                    AmmoBar.AddClass("invisible");
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
