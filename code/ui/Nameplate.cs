using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class Nameplate : Panel
    {
        public static float MaxDrawDistance = 500;

        private Label nameLabel;

        public Nameplate()
        {
            StyleSheet.Load("/ui/Nameplate.scss");

            nameLabel = Add.Label("", "name");
        }

        public override void Tick()
        {
            TTTPlayer player = Local.Pawn as TTTPlayer;

            TraceResult trace = Trace.Ray(player.EyePos, player.EyePos + player.EyeRot.Forward * MaxDrawDistance)
                .Ignore(player.ActiveChild)
                .Ignore(player)
                .UseHitboxes()
                .Run();

            bool validHit = false;

            if(trace.Hit && trace.Entity is TTTPlayer target)
            {
                validHit = true;

                nameLabel.Text = target.GetClientOwner()?.Name ?? "";
                nameLabel.Style.BackgroundColor = target.Role.Color;
                nameLabel.Style.Dirty();
            }

            SetClass("hide", !validHit);
        }
    }
}
