using Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

using TTTReborn.Player;

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
            bool traitorsDead = true;
            bool innocentsDead = true;

            // Check for alive players
            for (int i = 0; i < Client.All.Count; i++)
            {
                TTTPlayer alivePlayer = Client.All[i].Pawn as TTTPlayer;

                if (alivePlayer.LifeState != LifeState.Alive)
                {
                    continue;
                }

                if (alivePlayer.Role == TTTPlayer.RoleType.Traitor)
                {
                    traitorsDead = false;
                }
                else
                {
                    innocentsDead = false;
                }
            }

            // End this round if there is just one team alive
            return innocentsDead || traitorsDead;
        }
    }
}
