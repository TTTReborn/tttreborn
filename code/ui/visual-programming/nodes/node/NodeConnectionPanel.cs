namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionPanel : Panel
    {
        public NodeConnectionPoint ConnectionPoint;

        public NodeConnectionPanel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            ConnectionPoint = new(this);
        }
    }
}
