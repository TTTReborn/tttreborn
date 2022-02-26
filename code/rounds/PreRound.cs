using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;
using TTTReborn.Player;
using TTTReborn.Roles;
using TTTReborn.Settings;

namespace TTTReborn.Rounds
{
    public class PreRound : BaseRound
    {
        public override string RoundName => "Preparing";
        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.PreRoundTime;
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
                Gamemode.Game.Instance.MapHandler.Reset();

                foreach (Client client in Client.All)
                {
                    if (client.Pawn is TTTPlayer player)
                    {
                        player.RemoveLogicButtons();
                        player.Respawn();
                    }
                }
            }
        }

        protected override void OnTimeUp()
        {
            base.OnTimeUp();

            List<TTTPlayer> players = new();
            List<TTTPlayer> spectators = new();

            foreach (TTTPlayer player in Utils.GetPlayers())
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

        private static void AssignRolesAndRespawn(List<TTTPlayer> players)
        {
            VisualProgramming.NodeStack.Instance.Evaluate(new List<TTTPlayer>(players));

            foreach (TTTPlayer player in players)
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

        private static async void StartRespawnTimer(TTTPlayer player)
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

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            bool handsAdded = player.Inventory.TryAdd(new Hands(), deleteIfFails: true, makeActive: false);

            Log.Debug($"Attempting to add Hands to {player.Client.Name} {handsAdded}");

            base.OnPlayerSpawn(player);
        }
    }
}
