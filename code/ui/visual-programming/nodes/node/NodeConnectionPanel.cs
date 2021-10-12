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
            ConnectionPoints.Add(new(this));
        }
    }
}
