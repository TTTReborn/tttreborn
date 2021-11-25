// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

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
            }

            _staminaLabel.Text = playerController.Stamina.ToString("F0");

            SetClass("fade-in", playerController.Stamina < DefaultWalkController.MAX_STAMINA);
        }
    }
}
