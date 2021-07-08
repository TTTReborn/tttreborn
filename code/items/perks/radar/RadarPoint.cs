using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.UI;

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

            DistanceLabel = Add.Label((Local.Pawn as TTTPlayer)?.Position.Distance(Position).ToString(), "distance");

            Log.Warning($"{Position.ToString()}");
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            DistanceLabel.Text = player.Position.Distance(Position).ToString();

            Vector3 screenPos = Position.ToScreen();
            bool visible = screenPos.z > 0f;

            SetClass("hide", !visible);

            // visible
            if (visible)
            {
                Style.Left = Length.Fraction(screenPos.x);
                Style.Top = Length.Fraction(screenPos.y);
                Style.Dirty();
            }
        }
    }
}