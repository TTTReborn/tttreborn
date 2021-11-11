using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Map;
using TTTReborn.Player;
using TTTReborn.Settings;
using TTTReborn.UI;

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

            IDictionary<string, int> mapIndexToVoteCount = Map.MapSelection.GetTotalVotesPerMapIndex(Gamemode.Game.Instance.MapSelection.PlayerIdMapVote);
            if (mapIndexToVoteCount.Count == 0)
            {
                Global.ChangeLevel("facepunch.flatgrass");
            }
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            player.MakeSpectator();
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                RPCs.ClientOpenMapSelection();
            }
        }
    }
}
