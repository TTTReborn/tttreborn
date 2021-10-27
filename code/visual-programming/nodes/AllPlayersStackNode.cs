using TTTReborn.Globals;

namespace TTTReborn.VisualProgramming
{
    public partial class AllPlayersStackNode : StackNode
    {
        public AllPlayersStackNode() : base()
        {

        }

        public override object[] Build(params object[] input)
        {
            return base.Build(Utils.GetPlayers());
        }
    }
}
