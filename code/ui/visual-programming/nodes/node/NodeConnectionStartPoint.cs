using Sandbox;

namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionStartPoint : NodeConnectionPoint
    {
        public bool IsDragging { get; internal set; } = false;

        public NodeConnectionStartPoint() : base()
        {

        }

        protected override void OnMouseDown(Sandbox.UI.MousePanelEvent e)
        {
            if (ConnectionWire != null)
            {
                return;
            }

            ConnectionWire = NodeConnectionWire.Create();
            ConnectionWire.StartPoint = this;
            Window.Instance.ActiveNodeConnectionWire = ConnectionWire;
            IsDragging = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(Sandbox.UI.MousePanelEvent e)
        {
            if (!IsDragging)
            {
                return;
            }

            if (ConnectionWire.EndPoint == null)
            {
                ConnectionWire.Delete(true);
            }

            IsDragging = false;
            Window.Instance.ActiveNodeConnectionWire = null;

            base.OnMouseUp(e);
        }

        public override void Tick()
        {
            if (!IsDragging)
            {
                return;
            }

            ConnectionWire.UpdateMousePosition(Mouse.Position);

            base.Tick();
        }
    }
}
