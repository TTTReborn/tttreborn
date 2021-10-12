namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionWire : Panel
    {
        public NodeConnectionPoint StartPoint;

        public NodeConnectionPoint EndPoint;

        public NodeConnectionWire(Sandbox.UI.Panel parent = null) : base(parent)
        {
            AddClass("connectionwire");

            Style.Position = Sandbox.UI.PositionMode.Absolute;
        }

        protected override void OnRightClick(Sandbox.UI.MousePanelEvent e)
        {
            base.OnRightClick(e);

            Delete(true);
        }

        public override void OnDeleted()
        {
            if (StartPoint != null)
            {
                StartPoint.ConnectionWire = null;
            }

            if (EndPoint != null)
            {
                EndPoint.ConnectionWire = null;
            }

            base.OnDeleted();
        }

        public static NodeConnectionWire Create()
        {
            return new NodeConnectionWire(VisualProgrammingWindow.Instance.Content);
        }

        public void SetPos(Vector2 vector2)
        {
            Style.Left = Sandbox.UI.Length.Pixels(vector2.x);
            Style.Top = Sandbox.UI.Length.Pixels(vector2.y);
            Style.Dirty();
        }

        public void SetSize(Vector2 vector2)
        {
            Style.Width = Sandbox.UI.Length.Pixels(vector2.x);
            Style.Height = Sandbox.UI.Length.Pixels(vector2.y);
            Style.Dirty();
        }
    }
}
