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

            for (int i = 0; i < Node.NodeSettings.Count; i++)
            {
                NodeSetting nodeSetting = Node.NodeSettings[i];

                if (nodeSetting.Input?.ConnectionPoint == this)
                {
                    Node.ConnectionInputIds[i] = currentConnectionWire.StartPoint.Node.Id;
                }
            }

            for (int i = 0; i < currentConnectionWire.StartPoint.Node.NodeSettings.Count; i++)
            {
                NodeSetting nodeSetting = currentConnectionWire.StartPoint.Node.NodeSettings[i];

                if (nodeSetting.Output?.ConnectionPoint == currentConnectionWire.StartPoint)
                {
                    currentConnectionWire.StartPoint.Node.ConnectionOutputIds[i] = Node.Id;
                }
            }

            base.OnMouseOver(e);
        }
    }
}
