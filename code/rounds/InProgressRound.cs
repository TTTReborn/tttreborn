using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TTTReborn.Player;
using TTTReborn.Weapons;
using Game = TTTReborn.Gamemode.Game;

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

            if (IsRoundOver())
            {
                TTTReborn.Gamemode.Game.Instance.ChangeRound(new PostRound());
            }
        }

        public override void OnPlayerLeave(TTTPlayer player)
        {
            base.OnPlayerLeave(player);

            Spectators.Remove(player);

            if (IsRoundOver())
            {
                TTTReborn.Gamemode.Game.Instance.ChangeRound(new PostRound());
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

                    if (!Players.Contains(player))
                    {
                        AddPlayer(player);
                    }

                    // TODO: Remove once we can spawn in weapons into the map, for now just give the guns to people.
                    player.Inventory.DeleteContents();
                    player.Inventory.Add(new Shotgun(), true);
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
            TTTReborn.Gamemode.Game.Instance.ChangeRound(new PostRound());

            base.OnTimeUp();
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            player.MakeSpectator();

            Spectators.Add(player);
            Players.Remove(player);

            base.OnPlayerSpawn(player);
        }

        private bool IsRoundOver()
        {
            bool innocentsAlive = Players.Exists((player) => player.Role == TTTPlayer.RoleType.Innocent);
            bool terroristsAlive = Players.Exists((player) => player.Role == TTTPlayer.RoleType.Traitor);

            return !innocentsAlive || !terroristsAlive;
        }

        private void AssignRoles()
        {
            // TODO: There should be a neater way to handle this logic.
            Random random = new Random();

            int traitorCount = (int) Math.Max(Players.Count * 0.25f, 1f);
            for (int i = 0; i < traitorCount; i++)
            {
                List<TTTPlayer> unassignedPlayers = Players.Where(p => p.Role == TTTPlayer.RoleType.None).ToList();
                int randomId = random.Next(unassignedPlayers.Count);
                if (unassignedPlayers[randomId].Role == TTTPlayer.RoleType.None)
                {
                    unassignedPlayers[randomId].Role = TTTPlayer.RoleType.Traitor;
                }
            }

            foreach (TTTPlayer player in Players)
            {
                if (player.Role == TTTPlayer.RoleType.None)
                {
                    player.Role = TTTPlayer.RoleType.Innocent;
                }
            }
        }
    }
}
