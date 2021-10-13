namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionEndPoint : NodeConnectionPoint
    {
        public NodeConnectionEndPoint() : base()
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

            VisualProgrammingWindow.Instance.ActiveNodeConnectionWire = null;
            ConnectionWire.StartPoint.IsDragging = false;

            base.OnMouseOver(e);
        }
    }
}
