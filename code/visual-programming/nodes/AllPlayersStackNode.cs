namespace TTTReborn.VisualProgramming
{
    [StackNode("main")]
    public partial class AllPlayersStackNode : StackNode
    {
        public AllPlayersStackNode() : base()
        {

        }

        public override object[] Test(object[] input)
        {
            return new object[]
            {
                Utils.GetPlayers()
            };
        }

        public override object[] Evaluate(object[] input) => input;
    }
}
