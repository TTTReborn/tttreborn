using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;

namespace TTTReborn.UI
{
    public class MapSelectionMenu : Panel
    {
        public static MapSelectionMenu Instance;

        private readonly Panel _mapWrapper;
        private readonly List<MapPanel> _mapPanels;

        public MapSelectionMenu() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/mapselectionmenu/MapSelectionMenu.scss");

            AddClass("text-shadow");

            AddClass("background-color-secondary");
            AddClass("opacity-heavy");
            AddClass("fullscreen");

            Add.TranslationLabel("VOTE_NEXT_MAP", "title");

            _mapPanels = new();

            _mapWrapper = new Panel(this);
            _mapWrapper.AddClass("map-wrapper");

            InitMapPanels();

            Enabled = false;
        }

        [Event(Events.TTTEvent.MapSelectionHandler.MapImagesChange)]
        private void OnMapImagesChange()
        {
            InitMapPanels();
        }

        private void InitMapPanels()
        {
            IDictionary<string, string> mapImages = Gamemode.Game.Instance.MapSelection.MapImages;
            foreach (KeyValuePair<string, string> mapImage in mapImages)
            {
                if (_mapPanels.Exists((mapPanel) => mapPanel.MapName == mapImage.Key))
                {
                    continue;
                }

                MapPanel panel = new(mapImage.Key, mapImage.Value)
                {
                    Parent = _mapWrapper
                };

                _mapPanels.Add(panel);
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (!Enabled)
            {
                return;
            }

            IDictionary<long, string> playerIdMapVote = Gamemode.Game.Instance.MapSelection.PlayerIdMapVote;

            IDictionary<string, int> mapToVoteCount = Map.MapSelectionHandler.GetTotalVotesPerMap(playerIdMapVote);

            bool hasLocalClientVoted = playerIdMapVote.ContainsKey(Local.Client.PlayerId);

            for (int i = 0; i < _mapPanels.Count; ++i)
            {
                MapPanel panel = _mapPanels[i];

                panel.TotalVotes.Text = mapToVoteCount.ContainsKey(panel.MapName) ? $"{mapToVoteCount[panel.MapName]}" : string.Empty;

                panel.SetClass("voted", hasLocalClientVoted && playerIdMapVote[Local.Client.PlayerId] == panel.MapName);
            }
        }

        public class MapPanel : Panel
        {
            public string MapName;
            public Label TotalVotes;

            public MapPanel(string name, string image)
            {
                MapName = name;

                AddClass("box-shadow");
                AddClass("info-panel");
                AddClass("rounded");

                Add.Label(MapName, "map-name");
                TotalVotes = Add.Label(string.Empty, "map-vote");

                Style.BackgroundImage = Texture.Load(image);

                AddEventListener("onclick", () =>
                {
                    VoteNextMap(MapName);
                });
            }
        }

        [ServerCmd(Name = "vote_next_map")]
        public static void VoteNextMap(string name)
        {
            long callerPlayerId = ConsoleSystem.Caller.PlayerId;
            IDictionary<long, string> nextMapVotes = Gamemode.Game.Instance?.MapSelection.PlayerIdMapVote;

            // Remove previous vote if caller has already voted.
            if (nextMapVotes.ContainsKey(callerPlayerId))
            {
                nextMapVotes.Remove(callerPlayerId);
            }

            nextMapVotes[callerPlayerId] = name;
            Extensions.Log.Debug($"{callerPlayerId} voting for map index {name}");
        }
    }
}
