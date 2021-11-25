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

using TTTReborn.Player.Camera;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private const float MAX_HINT_DISTANCE = 2048;

        private EntityHintPanel _currentHintPanel;
        private IEntityHint _currentHint;

        private void TickEntityHints()
        {
            if (Camera is ThirdPersonSpectateCamera)
            {
                DeleteHint();

                return;
            }

            IEntityHint hint = IsLookingAtHintableEntity(MAX_HINT_DISTANCE);

            if (hint == null || !hint.CanHint(this))
            {
                DeleteHint();
                return;
            }

            if (hint == _currentHint)
            {
                hint.Tick(this);

                if (IsClient)
                {
                    _currentHintPanel.UpdateHintPanel(hint.TextOnTick);
                }

                return;
            }

            DeleteHint();

            if (IsClient)
            {
                if (hint.ShowGlow && hint is ModelEntity model && model.IsValid())
                {
                    model.GlowColor = Color.White; // TODO: Let's let people change this in their settings.
                    model.GlowActive = true;
                }

                _currentHintPanel = hint.DisplayHint(this);
                _currentHintPanel.Parent = HintDisplay.Instance;
                _currentHintPanel.Enabled = true;
            }

            _currentHint = hint;
        }

        private void DeleteHint()
        {
            if (IsClient)
            {
                if (_currentHint != null && _currentHint is ModelEntity model && model.IsValid())
                {
                    model.GlowActive = false;
                }

                _currentHintPanel?.Delete(true);
                _currentHintPanel = null;
            }

            _currentHint = null;
        }
    }
}
