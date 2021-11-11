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

            IDictionary<long, string> playerIdMapVote = Gamemode.Game.Instance?.MapSelection?.PlayerIdMapVote;
            IDictionary<string, int> mapToVoteCount = MapSelectionHandler.GetTotalVotesPerMap(playerIdMapVote);
            if (mapToVoteCount.Count == 0)
            {
                Global.ChangeLevel(ServerSettings.Instance?.Map?.DefaultMap ?? "facepunch.flatgrass");
            }

            Global.ChangeLevel(mapToVoteCount.OrderByDescending(x => x.Value).First().Key);
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            player.MakeSpectator();
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                RPCs.ClientOpenMapSelectionMenu();
            }
        }
    }
}
