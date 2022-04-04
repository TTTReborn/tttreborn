using Sandbox;
using Sandbox.UI;

#pragma warning disable IDE0052

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class BreathIndicator : Panel
    {
        public static BreathIndicator Instance { get; set; }

        private Panel BreathBar { get; set; }
        private string Breath { get; set; }

        public BreathIndicator()
        {
            Instance = this;
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not Player player || player.Controller is not DefaultWalkController playerController)
            {
                return;
            }

            if (playerController.Breath < DefaultWalkController.MAX_BREATH)
            {
                BreathBar.Style.Width = Length.Percent(playerController.Breath);
            }

            Breath = playerController.Breath.ToString("F0");

            SetClass("fade-in", playerController.Breath < DefaultWalkController.MAX_BREATH);
        }
    }
}
