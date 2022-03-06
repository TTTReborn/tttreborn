using TTTReborn.Player;

namespace TTTReborn.Teams
{
    [Team("nones")]
    public class NoneTeam : TTTTeam
    {
        public override Color Color => Color.Transparent;

        public NoneTeam() : base()
        {

        }

        public override bool CheckWin(TTTPlayer player) => false;
    }
}
