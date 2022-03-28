using System;

using Sandbox;

namespace TTTReborn.Rounds
{
    public class WaitingRound : BaseRound
    {
        public override string RoundName { get; set; } = "Waiting";

        public override void OnSecond()
        {
            if (Host.IsServer && Utils.HasMinimumPlayers())
            {
                Gamemode.Game.Instance.ForceRoundChange(new PreRound());
            }
        }

        public override void OnPlayerKilled(Player player)
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
                    if (client.Pawn is Player player)
                    {
                        player.Respawn();
                    }
                }
            }
        }

        private static async void StartRespawnTimer(Player player)
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
