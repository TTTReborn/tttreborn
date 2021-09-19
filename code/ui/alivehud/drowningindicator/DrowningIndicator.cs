using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class DrowningIndicator : Panel
    {
        public static DrowningIndicator Instance;

        private Panel _drowningBar;
        private Label _drowningLabel;

        public DrowningIndicator()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/drowningindicator/DrowningIndicator.scss");

            AddClass("text-shadow");

            _drowningBar = new(this);
            _drowningBar.AddClass("drowning-bar");
            _drowningBar.AddClass("center-horizontal");
            _drowningBar.AddClass("rounded");

            _drowningLabel = Add.Label();
            _drowningLabel.AddClass("drowning-label");
            _drowningLabel.Text = "O₂";

            Enabled = true;
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player || player.Controller is not DefaultWalkController playerController)
            {
                return;
            }

            float breathRemaining = playerController.UnderwaterBreathSeconds / DefaultWalkController.MAX_UNDERWATER_BREATH_SECONDS;

            if (breathRemaining < 1f)
            {
                _drowningBar.Style.Width = Length.Percent(breathRemaining * 100f);
                _drowningBar.Style.Dirty();
            }

            SetClass("fade-in", breathRemaining < 1f);
        }
    }
}
