using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TTTReborn.Player;
using TTTReborn.Weapons;

namespace TTTReborn.Rounds
{
    public class InProgressRound : BaseRound
    {
        public override string RoundName => "In Progress";
        public override int RoundDuration => TTTReborn.Gamemode.Game.TTTRoundTime;
        public override bool CanPlayerSuicide => true;

        public List<TTTPlayer> Spectators = new();

        private bool _isGameOver;

        public override void OnPlayerKilled(TTTPlayer player)
        {
            Players.Remove(player);
            Spectators.Add(player);

            player.MakeSpectator(player.EyePos);

            if (IsRoundOver())
            {
                _ = ChangeToPostRound();
            }
        }

        public override void OnPlayerLeave(TTTPlayer player)
        {
            base.OnPlayerLeave(player);

            Spectators.Remove(player);

            if (IsRoundOver())
            {
                _ = ChangeToPostRound();
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
            if (_isGameOver)
            {
                return;
            }

            _ = ChangeToPostRound();

            base.OnTimeUp();
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            player.MakeSpectator();

            Spectators.Add(player);
            Players.Remove(player);

            base.OnPlayerSpawn(player);
        }

        private async Task ChangeToPostRound(int delay = 3)
        {
            _isGameOver = true;

            await Task.Delay(delay * 1000);

            if (TTTReborn.Gamemode.Game.Instance.Round != this)
            {
                return;
            }

            TTTReborn.Gamemode.Game.Instance.ChangeRound(new PostRound());
        }

        private bool IsRoundOver()
        {
            bool innocentsAlive = Players.Exists((player) => player.Role == TTTPlayer.RoleType.Innocent);
            bool terroristsAlive = Players.Exists((player) => player.Role == TTTPlayer.RoleType.Traitor);

            return !innocentsAlive || !terroristsAlive;
        }

        private void AssignRoles()
        {
            // TODO: There might be a neater way to handle this logic.
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

            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Role == TTTPlayer.RoleType.None)
                {
                    Players[i].Role = TTTPlayer.RoleType.Innocent;
                }
            }
        }
    }
}
