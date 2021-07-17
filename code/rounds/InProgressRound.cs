using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Player;
using TTTReborn.Roles;
using TTTReborn.Teams;

namespace TTTReborn.Rounds
{
    public class InProgressRound : BaseRound
    {
        public override string RoundName => "In Progress";
        public override int RoundDuration => Gamemode.Game.TTTRoundTime;

        private readonly List<TTTPlayer> _spectators = new();

        public override void OnPlayerKilled(TTTPlayer player)
        {
            Players.Remove(player);
            _spectators.Add(player);

            player.MakeSpectator(player.EyePos);

            TTTTeam result = IsRoundOver();

            if (result != null)
            {
                LoadPostRound(result);
            }
        }

        public override void OnPlayerLeave(TTTPlayer player)
        {
            base.OnPlayerLeave(player);

            _spectators.Remove(player);

            TTTTeam result = IsRoundOver();

            if (result != null)
            {
                LoadPostRound(result);
            }
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                foreach (Client client in Client.All)
                {
                    if (client.Pawn is not TTTPlayer player)
                    {
                        continue;
                    }

                    if (player.LifeState == LifeState.Dead)
                    {
                        player.Respawn();
                    }
                    else
                    {
                        player.SetHealth(player.MaxHealth);
                    }

                    if (!Players.Contains(player))
                    {
                        AddPlayer(player);
                    }

                    #region Inventory Creation
                    Inventory inventory = player.Inventory as Inventory;
                    
                    inventory.TryAdd(new MagnetoStick(), true);

                    // Randomize between SMG and shotgun
                    if (new Random().Next() % 2 == 0)
                    {
                        if (inventory.TryAdd(new Shotgun(), false))
                        {
                            inventory.Ammo.Give(AmmoType.Buckshot, 16);
                        }
                    }
                    else
                    {
                        if (inventory.TryAdd(new SMG(), false))
                        {
                            inventory.Ammo.Give(AmmoType.SMG, 60);
                        }
                    }

                    if (inventory.TryAdd(new Pistol(), false))
                    {
                        inventory.Ammo.Give(AmmoType.Pistol, 30);
                    }
                    #endregion
                }

                AssignRoles();
            }
        }

        protected override void OnFinish()
        {
            if (Host.IsServer)
            {
                _spectators.Clear();
            }
        }

        protected override void OnTimeUp()
        {
            LoadPostRound(TTTTeam.GetTeam("Innocents"));

            base.OnTimeUp();
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            player.MakeSpectator();

            _spectators.Add(player);
            Players.Remove(player);

            base.OnPlayerSpawn(player);
        }

        private TTTTeam IsRoundOver()
        {
            List<TTTTeam> aliveTeams = new();

            foreach (TTTPlayer player in Players)
            {
                if (player.Team == null)
                {
                    continue;
                }

                if (!aliveTeams.Contains(player.Team))
                {
                    aliveTeams.Add(player.Team);
                }
            }

            return aliveTeams.Count == 1 ? aliveTeams[0] : null;
        }

        private void AssignRoles()
        {
            // TODO: There should be a neater way to handle this logic.
            Random random = new Random();

            int traitorCount = (int) Math.Max(Players.Count * 0.25f, 1f);

            for (int i = 0; i < traitorCount; i++)
            {
                List<TTTPlayer> unassignedPlayers = Players.Where(p => p.Role is NoneRole).ToList();
                int randomId = random.Next(unassignedPlayers.Count);

                if (unassignedPlayers[randomId].Role is NoneRole)
                {
                    unassignedPlayers[randomId].SetRole(new TraitorRole());
                }
            }

            foreach (TTTPlayer player in Players)
            {
                if (player.Role is NoneRole)
                {
                    player.SetRole(new InnocentRole());
                }

                // send everyone their roles
                using (Prediction.Off())
                {
                    RPCs.ClientSetRole(To.Single(player), player, player.Role.Name);
                }
            }
        }

        private static void LoadPostRound(TTTTeam winningTeam)
        {
            Gamemode.Game.Instance.ForceRoundChange(new PostRound());
            RPCs.ClientOpenAndSetPostRoundMenu(
                winningTeam.Name,
                winningTeam.Color
            );
        }

        private bool CheckMinimumPlayers()
        {
            return Client.All.Count >= Gamemode.Game.TTTMinPlayers;
        }

        public override void OnSecond()
        {
            if (Host.IsServer)
            {
                base.OnSecond();

                if (!Utils.HasMinimumPlayers())
                {
                    if (IsRoundOver() == null)
                    {
                        Gamemode.Game.Instance.ForceRoundChange(new WaitingRound());
                    }
                }
            }
        }
    }
}
