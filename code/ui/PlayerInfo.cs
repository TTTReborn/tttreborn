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

            new Background(this);
        }

        public class Background : Panel
        {
            public Label Weapon { set; get; }
            public Label Health { set; get; }

            public Background(Panel parent)
            {
                Parent = parent;

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
                    Weapon.Text = $"{weapon.AmmoClip}";
                }

                Health.Text = $"{player.Health:n0}";
            }
        }
    }

}
