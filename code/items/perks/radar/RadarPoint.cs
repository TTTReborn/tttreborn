using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class RadarPoint : Panel
    {
        private readonly Vector3 _position;
        private readonly Label _distanceLabel;
        private const int BLUR_RADIUS = 10;

        public RadarPoint(Radar.RadarPointData data)
        {
            _position = data.Position;

            StyleSheet.Load("/items/perks/radar/RadarPoint.scss");

            RadarDisplay.Instance.AddChild(this);

            AddClass("circular");

            _distanceLabel = Add.Label();
            _distanceLabel.AddClass("distance-label");
            _distanceLabel.AddClass("text-shadow");

            Style.BackgroundColor = data.Color;
            Style.BoxShadow = new ShadowList()
            {
                new Shadow
                {
                    Blur = BLUR_RADIUS,
                    Color = data.Color
                }
            };
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            _distanceLabel.Text = $"{Globals.Utils.SourceUnitsToMeters(player.Position.Distance(_position)):n0}m";

            Vector3 screenPos = _position.ToScreen();
            Enabled = screenPos.z > 0f;

            if (!Enabled)
            {
                return;
            }

            Style.Left = Length.Fraction(screenPos.x);
            Style.Top = Length.Fraction(screenPos.y);
            Style.Dirty();
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
