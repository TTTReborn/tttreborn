using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public class PostRound : BaseRound
    {
        public override string RoundName => "Post";
        public override int RoundDuration => 10;

        protected override void OnTimeUp()
        {
            TTTReborn.Gamemode.Game.Instance.ChangeRound(new PreRound());

            base.OnTimeUp();
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            if (Players.Contains(player)) return;

            player.MakeSpectator();

            AddPlayer(player);

            base.OnPlayerSpawn(player);
        }
    }
}
