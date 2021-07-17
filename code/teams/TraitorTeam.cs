namespace TTTReborn.Teams
{
    [TeamAttribute("Traitors")]
    public class TraitorTeam : TTTTeam
    {
        public override Color Color => Color.FromBytes(223, 41, 53);

        public TraitorTeam() : base()
        {

        }
    }
}
