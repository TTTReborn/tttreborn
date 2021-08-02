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

        public override void OnPlayerKilled(TTTPlayer player)
        {
            Players.Remove(player);
            Spectators.Add(player);

            player.MakeSpectator();

            TTTTeam result = IsRoundOver();

            if (result != null)
            {
                LoadPostRound(result);
            }
        }

        public override void OnPlayerLeave(TTTPlayer player)
        {
            base.OnPlayerLeave(player);

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

                    client.SetScore("forcedspectator", player.IsForcedSpectator);

                    if (player.LifeState == LifeState.Dead)
                    {
                        player.Respawn();
                    }
                    else
                    {
                        player.SetHealth(player.MaxHealth);
                    }

                    AddPlayer(player);

                    if (!player.IsForcedSpectator)
                    {
                        SetLoadout(player);
                    }
                }

                AssignRoles();
            }
        }

        protected override void OnTimeUp()
        {
            LoadPostRound(InnocentTeam.Instance);

            base.OnTimeUp();
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            if (player.IsForcedSpectator)
            {
                if (!Spectators.Contains(player))
                {
                    Spectators.Add(player);
                }

                Players.Remove(player);
            }
            else
            {
                if (!Players.Contains(player))
                {
                    Players.Add(player);
                }

                Spectators.Remove(player);

                SetLoadout(player);
            }

            base.OnPlayerSpawn(player);
        }

        private void SetLoadout(TTTPlayer player)
        {
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
            Random random = new Random();
            List<TTTPlayer> unassignedPlayers = Players.ToList();
            foreach (Type type in Globals.Utils.GetTypes<TTTRole>())
            {
                TTTRole role = Globals.Utils.GetObjectByType<TTTRole>(type);
                for (int i = 0; i < role.NumberOfPlayersWithRole(Players.Count); ++i)
                {
                    if (i < unassignedPlayers.Count)
                    {
                        int randomId = random.Next(unassignedPlayers.Count);
                        unassignedPlayers[randomId].SetRole(role);
                        unassignedPlayers.RemoveAt(randomId);
                    }
                }
            }

            foreach (TTTPlayer player in Players)
            {
                if (player.Role is NoneRole)
                {
                    player.SetRole(new InnocentRole());
                }

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
