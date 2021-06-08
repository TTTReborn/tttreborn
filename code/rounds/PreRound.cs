using Sandbox;
using System.Threading.Tasks;

using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public class PreRound : BaseRound
    {
        public override string RoundName => "Preparing";
        public override int RoundDuration => TTTReborn.Gamemode.Game.TTTPreRoundTime;

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

        protected override void OnTimeUp()
        {
            TTTReborn.Gamemode.Game.Instance.ChangeRound(new InProgressRound());

            base.OnTimeUp();
        }

        private async Task StartRespawnTimer(TTTPlayer player)
        {
            await Task.Delay(1000);

            if (TTTReborn.Gamemode.Game.Instance.Round is PreRound)
            {
                player.Respawn();
            }
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            if (Players.Contains(player))
            {
                return;
            }

            AddPlayer(player);

            base.OnPlayerSpawn(player);
        }
    }
}
