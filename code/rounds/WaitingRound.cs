using System.Threading.Tasks;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public class WaitingRound : BaseRound
    {
        public override string RoundName => "Waiting";

        public override void OnSecond()
        {
            if (Host.IsServer)
            {
                if (Utils.HasMinimumPlayers())
                {
                    Gamemode.Game.Instance.ForceRoundChange(new PreRound());
                }
            }
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            _ = StartRespawnTimer(player);

            base.OnPlayerKilled(player);
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                foreach (Client client in Client.All)
                {
                    if (client.Pawn is TTTPlayer player)
                    {
                        player.Respawn();
                    }
                }
            }
        }

        private static async Task StartRespawnTimer(TTTPlayer player)
        {
            await Task.Delay(1000);

            if (player.IsValid() && Gamemode.Game.Instance.Round is WaitingRound)
            {
                player.Respawn();
            }
        }
    }
}
