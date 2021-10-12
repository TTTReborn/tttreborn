using Sandbox;

namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionPoint : Panel
    {
        public bool IsDragging { get; private set; } = false;

        public NodeConnectionWire ConnectionWire { get; set; }

        private Vector2 _startPos;

        public NodeConnectionPoint(Sandbox.UI.Panel parent = null) : base(parent)
        {

        }

        protected override void OnMouseOver(Sandbox.UI.MousePanelEvent e)
        {
            NodeConnectionWire currentConnectionWire = VisualProgrammingWindow.Instance.ActiveNodeConnectionWire;

            if (ConnectionWire != null || currentConnectionWire == null)
            {
                return;
            }

            ConnectionWire = currentConnectionWire;
            ConnectionWire.EndPoint = this;

            base.OnMouseOver(e);
        }

        protected override void OnMouseDown(Sandbox.UI.MousePanelEvent e)
        {
            if (ConnectionWire != null)
            {
                return;
            }

            ConnectionWire = NodeConnectionWire.Create();
            ConnectionWire.StartPoint = this;
            VisualProgrammingWindow.Instance.ActiveNodeConnectionWire = ConnectionWire;
            _startPos = Mouse.Position;
            IsDragging = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(Sandbox.UI.MousePanelEvent e)
        {
            if (!IsDragging)
            {
                return;
            }

            VisualProgrammingWindow.Instance.ActiveNodeConnectionWire = null;
            IsDragging = false;

            if (ConnectionWire.EndPoint == null)
            {
                ConnectionWire.Delete(true);
                ConnectionWire = null;
            }

            base.OnMouseUp(e);
        }

        public override void Tick()
        {
            if (!IsDragging)
            {
                return;
            }

            Vector2 deltaVector = Mouse.Position - _startPos;

            ConnectionWire.SetPos(_startPos);
            ConnectionWire.SetSize(deltaVector);

            base.Tick();
        }
    }
}
