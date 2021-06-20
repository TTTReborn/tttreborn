using Sandbox;
using System.Threading.Tasks;

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
                CheckMinimumPlayers();
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

        private async Task StartRespawnTimer(TTTPlayer player)
        {
            await Task.Delay(1000);

            if (player.IsValid() && TTTReborn.Gamemode.Game.Instance.Round is WaitingRound)
            {
                player.Respawn();
            }
        }

        private void CheckMinimumPlayers()
        {
            if (Client.All.Count >= TTTReborn.Gamemode.Game.TTTMinPlayers)
            {
                TTTReborn.Gamemode.Game.Instance.ChangeRound(new PreRound());
            }
        }
    }
}
