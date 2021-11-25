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

namespace TTTReborn.Events
{
    public static partial class TTTEvent
    {
        public static class Game
        {
            /// <summary>
            /// Should be used to precache models and stuff.
            /// </summary>
            public const string Precache = "tttreborn.game.precache";

            /// <summary>
            /// Called everytime the round changes.
            /// <para>Event is passed the <strong><see cref="TTTReborn.Rounds.BaseRound"/></strong> instance of the old round.</para>
            /// <para>Event is passed the <strong><see cref="TTTReborn.Rounds.BaseRound"/></strong> instance of the new round.</para>
            /// </summary>
            public const string RoundChange = "tttreborn.game.roundchange";

            /// <summary>
            /// Updates when the map images are networked.
            /// </summary>
            public const string MapImagesChange = "tttreborn.game.mapimagechange";
        }
    }
}
