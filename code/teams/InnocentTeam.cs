namespace TTTReborn.Teams
{
    [TeamAttribute("Innocents")]
    public class InnocentTeam : TTTTeam
    {
        public override Color Color => Color.FromBytes(27, 197, 78);

        public InnocentTeam() : base()
        {

        }
    }
}
