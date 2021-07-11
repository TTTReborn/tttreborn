using System;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class DrowningIndicator : Panel
    {
        private BarPanel _drowningBar;

        public DrowningIndicator()
        {
            StyleSheet.Load("/ui/alivehud/DrowningIndicator.scss");

            _drowningBar = new BarPanel(this, "100", "drowningbar");

            SetClass("hide", true);
        }

        public override void Tick()
        {
            if (Local.Pawn is not TTTPlayer player
            || player.Controller is not DefaultWalkController defaultWalkController)
            {
                return;
            }

            if (defaultWalkController.IsDiving)
            {
                float leftDivingTime = MathF.Max(defaultWalkController.DrownDamageTime - defaultWalkController.TimeSinceDivingStarted, 0f);

                _drowningBar.TextLabel.Text = $"{leftDivingTime:n1}";

                _drowningBar.Style.Width = Length.Percent(leftDivingTime / defaultWalkController.DrownDamageTime * 100f);
                _drowningBar.Style.Dirty();
            }

            SetClass("hide", !defaultWalkController.IsDiving);
        }
    }
}
