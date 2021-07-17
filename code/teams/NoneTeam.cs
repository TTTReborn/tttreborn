namespace TTTReborn.Teams
{
    [TeamAttribute("Nones")]
    public class NoneTeam : TTTTeam
    {
        public override Color Color => Color.Transparent;

        public NoneTeam() : base()
        {

        }
    }
}
