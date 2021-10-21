using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class StaminaIndicator : Panel
    {
        public static StaminaIndicator Instance;

        private Panel _staminaBar;
        private Label _staminaLabel;

        public StaminaIndicator() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/staminaindicator/StaminaIndicator.scss");

            AddClass("text-shadow");

            _staminaBar = new(this);
            _staminaBar.AddClass("stamina-bar");
            _staminaBar.AddClass("center-horizontal");
            _staminaBar.AddClass("rounded");

            _staminaLabel = Add.Label();
            _staminaLabel.AddClass("stamina-label");

            Enabled = true;

            Style.ZIndex = -1;
            Style.Dirty();
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player || player.Controller is not DefaultWalkController playerController)
            {
                return;
            }

            if (playerController.Stamina < DefaultWalkController.MAX_STAMINA)
            {
                _staminaBar.Style.Width = Length.Percent(playerController.Stamina);
                _staminaBar.Style.Dirty();
            }

            _staminaLabel.Text = playerController.Stamina.ToString("F0");

            SetClass("fade-in", playerController.Stamina < DefaultWalkController.MAX_STAMINA);
        }
    }
}
