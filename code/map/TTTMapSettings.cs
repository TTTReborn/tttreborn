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

using TTTReborn.Events;
using TTTReborn.Rounds;

namespace TTTReborn.Map
{
    [Library("ttt_map_settings")]
    public partial class TTTMapSettings : Entity
    {
        /// <summary>
        /// Fired after PostLevelLoaded runs and MapSettings entity is found.
        /// </summary>
        protected Output SettingsSpawned { get; set; }
        /// <summary>
        /// Fired once Preround begins.
        /// </summary>
        protected Output RoundPreparation { get; set; }
        /// <summary>
        /// Fired once round starts and roles are assigned.
        /// </summary>
        protected Output RoundStart { get; set; }
        /// <summary>
        /// Fired once a win condition is met.
        /// </summary>
        protected Output RoundEnd { get; set; }

        /// <summary>
        /// Does not run on entity awake/spawn, is called explicitly by the TTT gamemode to trigger.
        /// </summary>
        public void FireSettingsSpawn() => SettingsSpawned.Fire(this);

        [Event(TTTEvent.Game.RoundChange)]
        private void FireRoundChange(BaseRound _, BaseRound newRound)
        {
            switch (newRound)
            {
                case PreRound:
                    RoundPreparation.Fire(this);

                    break;

                case InProgressRound:
                    RoundStart.Fire(this);

                    break;

                case PostRound:
                    RoundEnd.Fire(this);

                    break;
            }
        }
    }
}
