using System;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class DrowningIndicator : TTTPanel
    {
        private BarPanel _drowningBar;

        public DrowningIndicator()
        {
            StyleSheet.Load("/ui/alivehud/drowningindicator/DrowningIndicator.scss");

            _drowningBar = new(this, "100", "drowningbar");

            IsShowing = false;
        }

        public override void Tick()
        {
            if (Local.Pawn is not TTTPlayer player || player.Controller is not DefaultWalkController defaultWalkController)
            {
                return;
            }

            if (defaultWalkController.IsUnderwater)
            {
                float leftTimeUnderwater = MathF.Max(defaultWalkController.DurationUnderwaterUntilDamage - defaultWalkController.TimeSinceUnderwater, 0f);

                _drowningBar.TextLabel.Text = $"{leftTimeUnderwater:n1}";

                _drowningBar.Style.Width = Length.Percent(leftTimeUnderwater / defaultWalkController.DurationUnderwaterUntilDamage * 100f);
                _drowningBar.Style.Dirty();
            }

            IsShowing = defaultWalkController.IsUnderwater;
        }
    }
}
