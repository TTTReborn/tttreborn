namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionEndPoint : NodeConnectionPoint
    {
        public NodeConnectionEndPoint() : base()
        {

        }

        protected override void OnMouseOver(Sandbox.UI.MousePanelEvent e)
        {
            NodeConnectionWire currentConnectionWire = Window.Instance.ActiveNodeConnectionWire;

            if (ConnectionWire != null || currentConnectionWire == null || currentConnectionWire.StartPoint.Node == Node)
            {
                return;
            }

            ConnectionWire = currentConnectionWire;
            ConnectionWire.EndPoint = this;

            Window.Instance.ActiveNodeConnectionWire = null;
            ConnectionWire.StartPoint.IsDragging = false;

            base.OnMouseOver(e);
        }
    }
}
