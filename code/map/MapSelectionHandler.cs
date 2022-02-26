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
            Package result = await Package.Fetch(Global.GameIdent, true);

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

        public static void OnMapImagesChanged()
        {
            Event.Run(Events.TTTEvent.Game.MAP_IMAGES_CHANGE);
        }
    }
}
