using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class RadarPoint : Panel
    {
        public Vector3 Position;

        private Label DistanceLabel;

        public RadarPoint(Vector3 vector3)
        {
            Position = vector3;

            StyleSheet.Load("/items/perks/radar/RadarPoint.scss");

            Hud.Current.RootPanel.AddChild(this);

            DistanceLabel = Add.Label($"{(Local.Pawn as TTTPlayer)?.Position.Distance(Position):n0}", "distance");
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            DistanceLabel.Text = $"{player.Position.Distance(Position):n0}";

            Vector3 screenPos = Position.ToScreen();
            IsShowing = screenPos.z > 0f;

            if (IsShowing)
            {
                Style.Left = Length.Fraction(screenPos.x);
                Style.Top = Length.Fraction(screenPos.y);
                Style.Dirty();
            }
        }
    }
}
