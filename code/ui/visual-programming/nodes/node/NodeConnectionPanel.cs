using Sandbox.UI;

namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionPanel<T> : Panel where T : NodeConnectionPoint, new()
    {
        public Node Node
        {
            get => _node;
            set
            {
                _node = value;

                ConnectionPoint.Node = _node;
            }
        }
        private Node _node;

        public T ConnectionPoint;

        public NodeConnectionPanel(Panel parent = null) : base(parent)
        {
            ConnectionPoint = new();

            AddChild(ConnectionPoint);

            AddClass("nodeconnectionpanel");
        }
    }
}
