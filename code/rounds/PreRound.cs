using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Roles;
using TTTReborn.Settings;

namespace TTTReborn.Rounds
{
    public class PreRound : BaseRound
    {
        public override string RoundName { get; set; } = "Preparing";
        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.PreRoundTime;
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
                Gamemode.Game.Instance.MapHandler.Reset();

                foreach (Client client in Client.All)
                {
                    if (client.Pawn is Player player)
                    {
                        player.RemoveLogicButtons();
                        player.Respawn();
                    }
                }
            }

            base.OnStart();
        }

        protected override void OnTimeUp()
        {
            base.OnTimeUp();

            List<Player> players = new();
            List<Player> spectators = new();

            foreach (Player player in Utils.GetPlayers())
            {
                player.Client.SetValue("forcedspectator", player.IsForcedSpectator);

                if (player.IsForcedSpectator)
                {
                    player.MakeSpectator(false);
                    spectators.Add(player);
                }
                else
                {
                    players.Add(player);
                }
            }

            AssignRolesAndRespawn(players);

            Gamemode.Game.Instance.ChangeRound(new InProgressRound
            {
                Players = players,
                Spectators = spectators
            });
        }

        private static void AssignRolesAndRespawn(List<Player> players)
        {
            VisualProgramming.NodeStack.Instance.Evaluate(new List<Player>(players));

            foreach (Player player in players)
            {
                if (player.Role is NoneRole)
                {
                    player.SetRole(new InnocentRole());
                }

                using (Prediction.Off())
                {
                    player.SendClientRole();
                }

                if (player.LifeState == LifeState.Dead)
                {
                    player.Respawn();
                }
                else
                {
                    player.SetHealth(player.MaxHealth);
                }
            }
        }

        private static async void StartRespawnTimer(Player player)
        {
            try
            {
                await GameTask.DelaySeconds(1);

                if (player.IsValid() && Gamemode.Game.Instance.Round is PreRound)
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
