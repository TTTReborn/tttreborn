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

namespace TTTReborn.Settings
{
    public partial class ServerSettings
    {
        public Categories.Round Round { get; set; } = new();
        public Categories.Map Map { get; set; } = new();
        public Categories.AFK AFK { get; set; } = new();
        public Categories.Debug Debug { get; set; } = new();
    }

    namespace Categories
    {
        public partial class Round
        {
            [InputSetting]
            public int MinPlayers { get; set; } = 2; // The minimum players required to start.

            [InputSetting]
            public int PreRoundTime { get; set; } = 20; // The amount of time allowed for preparation.

            [InputSetting]
            public int RoundTime { get; set; } = 300; // The amount of time allowed for the main round.

            [InputSetting]
            public int PostRoundTime { get; set; } = 10; // The amount of time before the next round starts.

            [InputSetting]
            public int MapSelectionRoundTime { get; set; } = 15; // The amount of time to vote for the next map.

            [InputSetting]
            public int TotalRounds { get; set; } = 10; // The amount of rounds to play.

            [InputSetting]
            public int KillTimeReward { get; set; } = 30; // The amount of extra time given to traitors for killing an innocent.
        }

        public partial class Map
        {
            [InputSetting]
            public string DefaultMap { get; set; } = "facepunch.flatgrass"; // The map we choose by default.
        }

        public partial class AFK
        {
            [SwitchSetting]
            public bool ShouldKickPlayers { get; set; } = false; // If we should kick the players instead of forcing them as spectators.

            [InputSetting]
            public int SecondsTillKick { get; set; } = 180; // The amount of time before we determine a player is AFK.
        }

        public partial class Debug
        {
            [SwitchSetting]
            public bool PreventWin { get; set; } = false; // If the round should never end (for debugging purposes)
        }
    }
}
