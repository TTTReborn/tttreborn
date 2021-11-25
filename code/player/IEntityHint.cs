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

using TTTReborn.Globalization;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public interface IEntityHint
    {
        /// <summary>
        /// The max viewable distance of the hint.
        /// </summary>
        float HintDistance => 2048f;

        /// <summary>
        /// If we should show a glow around the entity.
        /// </summary>
        bool ShowGlow => true;

        /// <summary>
        /// The text to display on the hint each tick.
        /// </summary>
        TranslationData TextOnTick => null;

        /// <summary>
        /// Whether or not we can show the UI hint.
        /// </summary>
        bool CanHint(TTTPlayer client);

        /// <summary>
        /// The hint we should display.
        /// </summary>
        EntityHintPanel DisplayHint(TTTPlayer client);

        /// <summary>
        /// Occurs on each tick if the hint is active.
        /// </summary>
        void Tick(TTTPlayer player);
    }
}
