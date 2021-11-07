using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Map;
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
            _ = StartRespawnTimer(player);

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
                    continue;
                }

                players.Add(player);

                if (player.LifeState == LifeState.Dead)
                {
                    player.Respawn();
                }
                else
                {
                    player.SetHealth(player.MaxHealth);
                }
            }

            AssignRoles(players);

            Gamemode.Game.Instance.ChangeRound(new InProgressRound
            {
                Players = players,
                Spectators = spectators
            });
        }

        private void AssignRoles(List<TTTPlayer> players)
        {
            int traitorCount = (int) Math.Max(players.Count * 0.25f, 1f);

            for (int i = 0; i < traitorCount; i++)
            {
                List<TTTPlayer> unassignedPlayers = players.Where(p => p.Role is NoneRole).ToList();
                int randomId = Utils.RNG.Next(unassignedPlayers.Count);

                if (unassignedPlayers[randomId].Role is NoneRole)
                {
                    unassignedPlayers[randomId].SetRole(new TraitorRole());
                }
            }

            foreach (TTTPlayer player in players)
            {
                if (player.Role is NoneRole)
                {
                    player.SetRole(new InnocentRole());
                }

                // send everyone their roles
                using (Prediction.Off())
                {
                    player.SendClientRole();
                }
            }
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
            Extensions.Log.Debug($"Added loadout to {player.Client.Name}");

            player.Inventory.TryAdd(new Hands(), true);

            base.OnPlayerSpawn(player);
        }
    }
}
