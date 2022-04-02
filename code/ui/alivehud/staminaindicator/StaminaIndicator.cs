using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class StaminaIndicator : Panel
    {
        public static StaminaIndicator Instance;

        private Panel StaminaBar { get; set; }
        private string Stamina { get; set; }

        public StaminaIndicator() : base()
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

            if (playerController.Stamina < DefaultWalkController.MAX_STAMINA)
            {
                StaminaBar.Style.Width = Length.Percent(playerController.Stamina);
            }

            Stamina = playerController.Stamina.ToString("F0");

            SetClass("fade-in", playerController.Stamina < DefaultWalkController.MAX_STAMINA);
        }
    }
}
