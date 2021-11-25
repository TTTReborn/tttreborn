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
    public partial class BreathIndicator : Panel
    {
        public static BreathIndicator Instance;

        private Panel _breathBar;
        private Label _breathLabel;

        public BreathIndicator() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/breathindicator/BreathIndicator.scss");

            AddClass("text-shadow");

            _breathBar = new(this);
            _breathBar.AddClass("breath-bar");
            _breathBar.AddClass("center-horizontal");
            _breathBar.AddClass("rounded");

            _breathLabel = Add.Label();
            _breathLabel.AddClass("breath-label");

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

            if (playerController.Breath < DefaultWalkController.MAX_BREATH)
            {
                _breathBar.Style.Width = Length.Percent(playerController.Breath);
            }

            _breathLabel.Text = playerController.Breath.ToString("F0");

            SetClass("fade-in", playerController.Breath < DefaultWalkController.MAX_BREATH);
        }
    }
}
