using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Gamemode;
using TTTReborn.Weapons;
using TTTReborn.Player;

namespace TTTReborn.UI
{
public class PlayerInfo : Panel
{
    public Label Weapon { set; get; }
    public Label Health { set; get; }

    public PlayerInfo()
    {
        StyleSheet.Load("/ui/PlayerInfo.scss");

        Weapon = Add.Label("100", "weapon");
        Health = Add.Label("100", "health");
    }

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
            //Weapon.SetClass("active", weapon != null);

            Weapon.Text = $"{weapon.AmmoClip}";
        }

        Health.Text = $"{player.Health:n0}";

        SetClass("active", true);
    }
}

}
