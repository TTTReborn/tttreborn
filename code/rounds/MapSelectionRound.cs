using System.Collections.Generic;
using System.Linq;

using TTTReborn.Map;
using TTTReborn.Settings;

namespace TTTReborn.Rounds
{
    public class MapSelectionRound : BaseRound
    {
        public override string RoundName { get; set; } = "Map Selection";

        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.MapSelectionRoundTime;
        }

        protected override void OnTimeUp()
        {
            base.OnTimeUp();

            IDictionary<string, string> maps = Gamemode.TTTGame.Instance.MapSelection.MapImages;

            // We failed to fetch TTT maps, fall back to default map.
            if (maps.Count == 0)
            {
                Sandbox.Game.ChangeLevel(ServerSettings.Instance.Map.DefaultMap);

                return;
            }

            IDictionary<long, string> playerIdMapVote = Gamemode.TTTGame.Instance.MapSelection.PlayerIdMapVote;
            IDictionary<string, int> mapToVoteCount = MapSelectionHandler.GetTotalVotesPerMap(playerIdMapVote);

            // Nobody voted, so let's change to a random map.
            if (mapToVoteCount.Count == 0)
            {
                Sandbox.Game.ChangeLevel(maps.ElementAt(Utils.RNG.Next(maps.Count)).Key);

                return;
            }

            // Change to the map which received the most votes first.
            Sandbox.Game.ChangeLevel(mapToVoteCount.OrderByDescending(x => x.Value).First().Key);
        }

        public override void OnPlayerKilled(Player player)
        {
            player.MakeSpectator();

            base.OnPlayerKilled(player);
        }

        protected override void OnStart()
        {
            RPCs.ClientOpenMapSelectionMenu();

            base.OnStart();
        }
    }
}
