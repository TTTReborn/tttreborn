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

            RadarDisplay.Instance.AddChild(this);

            AddClass("circular");

            DistanceLabel = Add.Label();
            DistanceLabel.AddClass("distance-label");
            DistanceLabel.AddClass("text-shadow");
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            DistanceLabel.Text = $"{Globals.Utils.SourceUnitsToMeters(player.Position.Distance(Position)):n0}m";

            Vector3 screenPos = Position.ToScreen();
            Enabled = screenPos.z > 0f;

            if (Enabled)
            {
                Style.Left = Length.Fraction(screenPos.x);
                Style.Top = Length.Fraction(screenPos.y);
                Style.Dirty();
            }
        }
    }

    public class RadarDisplay : Panel
    {
        public static RadarDisplay Instance { get; set; }

        public RadarDisplay() : base()
        {
            Instance = this;

            AddClass("fullscreen");

            Style.ZIndex = -1;
            Style.Dirty();
        }
    }
}
