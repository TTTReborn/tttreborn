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

        private Panel _labelHolder;
        private readonly Label _nameLabel;
        private Label _damageIndicatorLabel;

        public Nameplate()
        {
            StyleSheet.Load("/ui/Nameplate.scss");

            _labelHolder = Add.Panel("labelHolder");
            _nameLabel = _labelHolder.Add.Label("", "name");
            _damageIndicatorLabel = _labelHolder.Add.Label("", "damageIndicator");

        }

        private string GetHealthGroup(float health)
        {
            return health > 70 ? "Healthy"
                : health > 20 ? "Injured"
                : "Near death";
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
                _damageIndicatorLabel.Text = GetHealthGroup(target.Health);
                _labelHolder.Style.BackgroundColor = target.Role.Color.WithAlpha(0.5f);
                _labelHolder.Style.Dirty();
            }

            SetClass("hide", !validHit);
        }
    }
}
