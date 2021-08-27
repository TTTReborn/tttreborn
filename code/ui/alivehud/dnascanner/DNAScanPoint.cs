using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class DNAScanPoint : TTTPanel
    {
        public Vector3 Position { get; private set; }

        private readonly Label _distanceLabel;

        public DNAScanPoint(Vector3 position)
        {
            Position = position;

            StyleSheet.Load("/ui/alivehud/dnascanner/DNAScanPoint.scss");

            Hud.Current.RootPanel.AddChild(this);

            _distanceLabel = Add.Label($"{(Local.Pawn as TTTPlayer)?.Position.Distance(Position):n0}", "distance");
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            _distanceLabel.Text = $"{player.Position.Distance(Position):n0}";

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
