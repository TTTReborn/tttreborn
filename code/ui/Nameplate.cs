using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class Nameplate : Panel
    {
        private const float MAX_DRAW_DISTANCE = 500;

        private readonly Label _nameLabel;

        public Nameplate()
        {
            StyleSheet.Load("/ui/Nameplate.scss");

            _nameLabel = Add.Label("", "name");
        }

        public override void Tick()
        {
            TTTPlayer player = Local.Pawn as TTTPlayer;

            TraceResult trace = Trace.Ray(player.EyePos, player.EyePos + player.EyeRot.Forward * MAX_DRAW_DISTANCE)
                .Ignore(player.ActiveChild)
                .Ignore(player)
                .UseHitboxes()
                .Run();

            bool validHit = false;

            if (trace.Hit && trace.Entity is TTTPlayer target)
            {
                validHit = true;

                _nameLabel.Text = target.GetClientOwner()?.Name ?? "";
                _nameLabel.Style.BackgroundColor = target.Role.Color.WithAlpha(0.5f);
                _nameLabel.Style.Dirty();
            }

            SetClass("hide", !validHit);
        }
    }
}
