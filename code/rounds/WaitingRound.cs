using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public class WaitingRound : BaseRound
    {
        public override string RoundName => "Waiting";

        public override void OnSecond()
        {
            if (Host.IsServer && Utils.HasMinimumPlayers())
            {
                Gamemode.Game.Instance.ForceRoundChange(new PreRound());
            }
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            StartRespawnTimer(player);

            player.MakeSpectator();

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

        private static async void StartRespawnTimer(TTTPlayer player)
        {
            try
            {
                await GameTask.DelaySeconds(1);

                if (player.IsValid() && Gamemode.Game.Instance.Round is WaitingRound)
                {
                    player.Respawn();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Trim() == "A task was canceled.")
                {
                    return;
                }

                Log.Error($"[TASK] {e.Message}: {e.StackTrace}");
            }
        }
    }
}
