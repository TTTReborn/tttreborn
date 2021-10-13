using System.Collections.Generic;

namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionPanel : Panel
    {
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

        public List<NodeConnectionPoint> ConnectionPoints = new();

        public NodeConnectionPanel(Sandbox.UI.Panel parent = null) : base(parent)
        {

        }

        public T AddConnectionPoint<T>() where T : NodeConnectionPoint, new()
        {
            T connectionPoint = new();

            AddChild(connectionPoint);
            ConnectionPoints.Add(connectionPoint);

            return connectionPoint;
        }

        public NodeConnectionPoint GetConnectionPoint(int index = 0)
        {
            if (index >= ConnectionPoints.Count)
            {
                return null;
            }

            return ConnectionPoints[index];
        }
    }
}
