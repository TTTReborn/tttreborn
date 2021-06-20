using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

using TTTReborn.Player;
using TTTReborn.Items;
using TTTReborn.Roles;
using TTTReborn.Teams;

namespace TTTReborn.Rounds
{
    public class InProgressRound : BaseRound
    {
        public override string RoundName => "In Progress";
        public override int RoundDuration => TTTReborn.Gamemode.Game.TTTRoundTime;
        public override bool CanPlayerSuicide => true;

        public List<TTTPlayer> Spectators = new();

        public override void OnPlayerKilled(TTTPlayer player)
        {
            Players.Remove(player);
            Spectators.Add(player);

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

            Spectators.Remove(player);

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

                    // TODO: Remove once we can spawn in weapons into the map, for now just give the guns to people.
                    player.Inventory.Add(new Shotgun(), true);
                    player.GiveAmmo(AmmoType.Buckshot, 16);

                    player.Inventory.Add(new SMG(), false);
                    player.Inventory.Add(new Pistol(), false);
                    player.GiveAmmo(AmmoType.Pistol, 120);
                }

                AssignRoles();
            }
        }

        protected override void OnFinish()
        {
            if (Host.IsServer)
            {
                Spectators.Clear();
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

            Spectators.Add(player);
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
                using(Prediction.Off())
                {
                    player.ClientSetRole(To.Single(player), player.Role.Name);
                }
            }
        }

        private static void LoadPostRound(TTTTeam winningTeam)
        {
            Gamemode.Game.Instance.ChangeRound(new PostRound());
            TTTPlayer.ClientOpenAndSetPostRoundMenu(
                winningTeam.Name,
                winningTeam.Color
            );
        }
    }
}
