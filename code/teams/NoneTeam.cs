using TTTReborn.Player;

namespace TTTReborn.Teams
{
    [Team("nones")]
    public class NoneTeam : Team
    {
        public override Color Color => Color.Transparent;

        public NoneTeam() : base()
        {

        }

        public override bool CheckWin(TTTPlayer player) => false;
    }
}
