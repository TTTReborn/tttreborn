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

        public new bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                SetClass("disable", !_enabled);
            }
        }
        private bool _enabled = true;

        public T ConnectionPoint;

        public NodeConnectionPanel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            ConnectionPoint = new();

            AddChild(ConnectionPoint);

            AddClass("nodeconnectionpanel");
        }
    }
}
