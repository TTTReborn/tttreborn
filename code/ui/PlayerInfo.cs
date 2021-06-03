using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Weapons;
using TTTReborn.Player;

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

        public RolePanel(Panel parent)
        {
            Parent = parent;

            RoleLabel = Add.Label("Unknown role", "rolelabel");
        }

        /*
        public override void Tick()
        {
            TTTPlayer player = Local.Pawn as TTTPlayer;

            if (player == null)
            {
                return;
            }

            var weapon = player.ActiveChild as Weapon;

            if (weapon != null)
            {
                Weapon.Text = $"{weapon.AmmoClip}";
            }

            Health.Text = $"{player.Health:n0}";
        }
        */
    }

    public class IndicatorsPanel : Panel
    {
        public BarPanel HealthBar { set; get; }
        public BarPanel AmmoBar { set; get; }

        public IndicatorsPanel(Panel parent)
        {
            Parent = parent;

            HealthBar = new BarPanel(this, "100", "healthlabel");
            HealthBar.AddClass("health");

            AmmoBar = new BarPanel(this, "7/21", "ammolabel");
            AmmoBar.AddClass("ammo");
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
