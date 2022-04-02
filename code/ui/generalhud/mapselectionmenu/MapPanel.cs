using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class MapPanel : Panel
    {
        public string MapName { get; set; }
        public Label TotalVotes { get; set; }

        public MapPanel(string name, string image)
        {
            MapName = name;
            Style.BackgroundImage = Texture.Load(image);

            AddEventListener("onclick", () =>
            {
                MapSelectionMenu.VoteNextMap(MapName);
            });
        }
    }
}
