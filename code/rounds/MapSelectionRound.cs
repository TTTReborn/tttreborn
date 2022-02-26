using System.Collections.Generic;
using System.Linq;

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
