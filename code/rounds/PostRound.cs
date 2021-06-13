using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public class PostRound : BaseRound
    {
        public override string RoundName => "Post";
        public override int RoundDuration => TTTReborn.Gamemode.Game.TTTPostRoundTime;

        protected override void OnTimeUp()
        {
            TTTReborn.Gamemode.Game.Instance.ChangeRound(new PreRound());

            base.OnTimeUp();
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            if (Players.Contains(player))
            {
                return;
            }

            player.MakeSpectator();

            AddPlayer(player);

            base.OnPlayerSpawn(player);
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                using(Prediction.Off())
                {
                    foreach (TTTPlayer player in TTTPlayer.GetAll())
                    {
                        player.ClientSetRole(player.Role.Name);
                    }
                }
            }
        }
    }
}
