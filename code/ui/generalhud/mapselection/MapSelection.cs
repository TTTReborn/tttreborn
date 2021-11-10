using System.Collections.Generic;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class MapSelection : Panel
    {
        private readonly Panel _mapWrapper;

        public MapSelection() : base()
        {
            StyleSheet.Load("/ui/generalhud/mapselection/MapSelection.scss");

            AddClass("text-shadow");

            AddClass("background-color-secondary");
            AddClass("opacity-heavy");
            AddClass("fullscreen");

            Add.TranslationLabel("VOTE_NEXT_MAP", "title");

            _mapWrapper = new Panel(this);
            _mapWrapper.AddClass("map-wrapper");

            InitMapPanels();
        }

        private async void InitMapPanels()
        {
            List<string> mapNames = await GetTTTMapNames();
            List<MapPanel> mapPanels = await GetTTTMapPanels(mapNames);
            mapPanels.ForEach((mapPanel) =>
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
                mapPanels.Add(new MapPanel(mapName, result.Thumb));
            }
            return mapPanels;
        }

        public override void Tick()
        {
            base.Tick();
        }

        public class MapPanel : Panel
        {
            public Label TotalVotes;

            public MapPanel(string name, string image)
            {
                TotalVotes = Add.Label("0", "map-votes");
                Add.Label(name, "map-name");

                Style.BackgroundImage = Texture.Load(image);

                AddEventListener("onclick", () =>
                {

                });
            }
        }
    }
}
