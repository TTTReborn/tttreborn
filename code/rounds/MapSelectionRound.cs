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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Map;
using TTTReborn.Player;
using TTTReborn.Settings;

namespace TTTReborn.Rounds
{
    public class MapSelectionRound : BaseRound
    {
        public override string RoundName => "Map Selection";
        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.MapSelectionRoundTime;
        }

        protected override void OnTimeUp()
        {
            base.OnTimeUp();

            IDictionary<string, string> maps = Gamemode.Game.Instance.MapSelection.MapImages;

            // We failed to fetch TTT maps, fall back to default map.
            if (maps.Count == 0)
            {
                Global.ChangeLevel(ServerSettings.Instance.Map.DefaultMap);
                return;
            }

            IDictionary<long, string> playerIdMapVote = Gamemode.Game.Instance.MapSelection.PlayerIdMapVote;
            IDictionary<string, int> mapToVoteCount = MapSelectionHandler.GetTotalVotesPerMap(playerIdMapVote);

            // Nobody voted, so let's change to a random map.
            if (mapToVoteCount.Count == 0)
            {
                Global.ChangeLevel(maps.ElementAt(Utils.RNG.Next(maps.Count)).Key);
                return;
            }

            // Change to the map which received the most votes first.
            Global.ChangeLevel(mapToVoteCount.OrderByDescending(x => x.Value).First().Key);
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            player.MakeSpectator();
        }

        protected override void OnStart()
        {
            RPCs.ClientOpenMapSelectionMenu();
        }
    }
}
