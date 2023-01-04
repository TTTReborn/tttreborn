using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class MapSelectionMenu : Panel
    {
        public static MapSelectionMenu Instance { get; set; }

        private TranslationLabel TitleLabel { get; set; }
        private Panel MapWrapper { get; set; }

        public MapSelectionMenu() : base()
        {
            Instance = this;

            TitleLabel.UpdateTranslation(new TranslationData("MAPSELECTION.VOTE"));

            InitMapPanels();

            this.Enabled(false);
        }

        [Event("game_mapimagechange")]
        protected void OnMapImagesChange()
        {
            InitMapPanels();
        }

        private List<MapPanel> GetMapPanels()
        {
            List<MapPanel> panels = new();

            foreach (Panel panel in MapWrapper.Children)
            {
                if (panel is MapPanel mapPanel)
                {
                    panels.Add(mapPanel);
                }
            }

            return panels;
        }

        private void InitMapPanels()
        {
            IDictionary<string, string> mapImages = Gamemode.TTTGame.Instance.MapSelection.MapImages;

            List<MapPanel> mapPanels = GetMapPanels();

            foreach (KeyValuePair<string, string> mapImage in mapImages)
            {
                if (mapPanels.Exists((mapPanel) => mapPanel.MapName == mapImage.Key))
                {
                    continue;
                }

                MapPanel panel = new(mapImage.Key, mapImage.Value)
                {
                    Parent = MapWrapper
                };
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (!this.IsEnabled())
            {
                return;
            }

            IDictionary<long, string> playerIdMapVote = Gamemode.TTTGame.Instance.MapSelection.PlayerIdMapVote;
            IDictionary<string, int> mapToVoteCount = Map.MapSelectionHandler.GetTotalVotesPerMap(playerIdMapVote);

            bool hasLocalClientVoted = playerIdMapVote.ContainsKey(Game.LocalClient.SteamId);

            GetMapPanels().ForEach((mapPanel) =>
            {
                mapPanel.TotalVotes.Text = mapToVoteCount.ContainsKey(mapPanel.MapName) ? $"{mapToVoteCount[mapPanel.MapName]}" : string.Empty;

                mapPanel.SetClass("voted", hasLocalClientVoted && playerIdMapVote[Game.LocalClient.SteamId] == mapPanel.MapName);
            });
        }

        [ConCmd.Server(Name = "ttt_vote_next_map")]
        public static void VoteNextMap(string name)
        {
            long callerPlayerId = ConsoleSystem.Caller.SteamId;
            IDictionary<long, string> nextMapVotes = Gamemode.TTTGame.Instance.MapSelection.PlayerIdMapVote;

            nextMapVotes[callerPlayerId] = name;

            Log.Debug($"{callerPlayerId} voting for map {name}");
        }

        [ConCmd.Admin(Name = "ttt_test_vote")]
        public static void TestVote()
        {
            RPCs.ClientOpenMapSelectionMenu();
        }
    }
}
