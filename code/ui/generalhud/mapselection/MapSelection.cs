using System.Collections.Generic;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class MapSelection : Panel
    {
        public static MapSelection Instance;

        private readonly Panel _mapWrapper;
        private List<MapPanel> _mapPanels;

        public MapSelection() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/mapselection/MapSelection.scss");

            AddClass("text-shadow");

            AddClass("background-color-secondary");
            AddClass("opacity-heavy");
            AddClass("fullscreen");

            Add.TranslationLabel("VOTE_NEXT_MAP", "title");

            _mapWrapper = new Panel(this);
            _mapWrapper.AddClass("map-wrapper");

            InitMapPanels();

            Enabled = false;
        }

        private async void InitMapPanels()
        {
            List<string> mapNames = await GetTTTMapNames();
            _mapPanels = await GetTTTMapPanels(mapNames);
            _mapPanels.ForEach((mapPanel) =>
            {
                mapPanel.Parent = _mapWrapper;
            });
        }

        private static async Task<List<string>> GetTTTMapNames()
        {
            Package result = await Package.Fetch(Global.GameName, true);
            return result.GameConfiguration.MapList;
        }

        private static async Task<List<MapPanel>> GetTTTMapPanels(List<string> mapNames)
        {
            List<MapPanel> mapPanels = new();
            for (int i = 0; i < mapNames.Count; ++i)
            {
                string mapName = mapNames[i];
                Package result = await Package.Fetch(mapName, true);
                mapPanels.Add(new MapPanel(mapName, result.Thumb, i));
            }
            return mapPanels;
        }

        public override void Tick()
        {
            base.Tick();

            if (!Enabled)
            {
                return;
            }

            IDictionary<long, int> nextMapVotes = Gamemode.Game.Instance.NextMapVotes;
            if (nextMapVotes == null)
            {
                return;
            }

            // Count how many votes each map has.
            IDictionary<int, int> indexToVoteCount = new Dictionary<int, int>();
            foreach (int index in nextMapVotes.Values)
            {
                indexToVoteCount[index] = !indexToVoteCount.ContainsKey(index) ? 1 : indexToVoteCount[index]++;
            }

            bool hasLocalClientVoted = nextMapVotes.ContainsKey(Local.Client.PlayerId);

            // Iterate over the map panels, update vote total, showcase the current map the player has selected.
            for (int i = 0; i < _mapPanels.Count; ++i)
            {
                MapPanel panel = _mapPanels[i];

                panel.TotalVotes.Text = indexToVoteCount.ContainsKey(i) ? indexToVoteCount[i] == 1 ? $"{1} vote" : $"{indexToVoteCount[i]} votes" : "";

                panel.SetClass("voted", hasLocalClientVoted && nextMapVotes[Local.Client.PlayerId] == i);
            }
        }

        public class MapPanel : Panel
        {
            public Label TotalVotes;

            public MapPanel(string name, string image, int index)
            {
                AddClass("box-shadow");
                AddClass("info-panel");
                AddClass("rounded");

                Add.Label(name, "map-name text-color-info");
                TotalVotes = Add.Label("0", "map-vote text-color-info");

                Style.BackgroundImage = Texture.Load(image);

                AddEventListener("onclick", () =>
                {
                    VoteNextMap(index);
                });
            }
        }

        [ServerCmd(Name = "vote_next_map")]
        public static void VoteNextMap(int mapIndex)
        {
            long callerPlayerId = ConsoleSystem.Caller.PlayerId;
            IDictionary<long, int> nextMapVotes = Gamemode.Game.Instance?.NextMapVotes;

            // Remove previous vote if caller has already voted.
            if (nextMapVotes.ContainsKey(callerPlayerId))
            {
                nextMapVotes.Remove(callerPlayerId);
            }

            nextMapVotes[callerPlayerId] = mapIndex;
            Extensions.Log.Debug($"{callerPlayerId} voting for map index {mapIndex}");
        }
    }
}
