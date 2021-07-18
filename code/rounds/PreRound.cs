using System.Threading.Tasks;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public class PreRound : BaseRound
    {
        public override string RoundName => "Preparing";
        public override int RoundDuration => Gamemode.Game.TTTPreRoundTime;

        public override void OnPlayerKilled(TTTPlayer player)
        {
            _ = StartRespawnTimer(player);

            player.MakeSpectator();

            base.OnPlayerKilled(player);
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                foreach (Entity entity in Entity.All)
                {
                    if (entity is BaseCarriable carr)
                    {
                        carr.Delete();
                    }
                }

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
            base.OnTimeUp();

            Gamemode.Game.Instance.ChangeRound(new InProgressRound());
        }

        private static async Task StartRespawnTimer(TTTPlayer player)
        {
            await Task.Delay(1000);

            if (player.IsValid() && Gamemode.Game.Instance.Round is PreRound)
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
