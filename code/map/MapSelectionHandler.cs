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
using System.Threading.Tasks;

using Sandbox;

namespace TTTReborn.Map
{
    public partial class MapSelectionHandler : BaseNetworkable
    {
        // Player Id (long) -> Map name (string)
        [Net]
        public IDictionary<long, string> PlayerIdMapVote { get; set; }

        // Map name (string) -> Map image (string)
        [Net, Change]
        public IDictionary<string, string> MapImages { get; set; }

        public int TotalRoundsPlayed = 0;

        public async Task Load()
        {
            List<string> mapNames = await GetTTTMapNames();
            List<string> mapImages = await GetTTTMapImages(mapNames);

            for (int i = 0; i < mapNames.Count; ++i)
            {
                MapImages[mapNames[i]] = mapImages[i];
            }
        }

        private static async Task<List<string>> GetTTTMapNames()
        {
            Package result = await Package.Fetch(Global.GameName, true);
            return result.GameConfiguration.MapList;
        }

        private static async Task<List<string>> GetTTTMapImages(List<string> mapNames)
        {
            List<string> mapPanels = new();
            for (int i = 0; i < mapNames.Count; ++i)
            {
                Package result = await Package.Fetch(mapNames[i], true);
                mapPanels.Add(result.Thumb);
            }
            return mapPanels;
        }

        public static IDictionary<string, int> GetTotalVotesPerMap(IDictionary<long, string> mapVotes)
        {
            IDictionary<string, int> indexToVoteCount = new Dictionary<string, int>();
            foreach (string mapName in mapVotes.Values)
            {
                indexToVoteCount[mapName] = !indexToVoteCount.ContainsKey(mapName) ? 1 : indexToVoteCount[mapName] + 1;
            }
            return indexToVoteCount;
        }

        public void OnMapImagesChanged()
        {
            Event.Run(Events.TTTEvent.Game.MapImagesChange);
        }
    }
}
