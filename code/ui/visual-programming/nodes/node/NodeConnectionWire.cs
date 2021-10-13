namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionWire : Panel
    {
        public NodeConnectionStartPoint StartPoint;
        public NodeConnectionEndPoint EndPoint;

        private Vector2 _startPos;
        private Vector2 _endPos;

        public NodeConnectionWire(Sandbox.UI.Panel parent = null) : base(parent)
        {
            AddClass("connectionwire");

            Style.Position = Sandbox.UI.PositionMode.Absolute;
        }

        protected override void OnRightClick(Sandbox.UI.MousePanelEvent e)
        {
            base.OnRightClick(e);

            if (StartPoint.IsDragging)
            {
                return;
            }

            Delete(true);
        }

        public override void Delete(bool immediate = false)
        {
            if (StartPoint != null)
            {
                StartPoint.ConnectionWire = null;
            }

            if (EndPoint != null)
            {
                EndPoint.ConnectionWire = null;
            }

            base.Delete(immediate);
        }

        public static NodeConnectionWire Create()
        {
            return new NodeConnectionWire(VisualProgrammingWindow.Instance.Content);
        }

        public void UpdateMousePosition(Vector2 vector2)
        {
            Style.Left = Sandbox.UI.Length.Pixels(StartPoint.Box.Rect.left);
            Style.Top = Sandbox.UI.Length.Pixels(StartPoint.Box.Rect.top);
            Style.Width = Sandbox.UI.Length.Pixels(vector2.x - StartPoint.Box.Rect.left);
            Style.Height = Sandbox.UI.Length.Pixels(vector2.y - StartPoint.Box.Rect.top);
            Style.Dirty();
        }

        public override void Tick()
        {
            base.Tick();

            if (StartPoint == null || EndPoint == null)
            {
                return;
            }

            Vector2 startPos = StartPoint.Position;
            Vector2 endPos = EndPoint.Position;

            if (startPos != _startPos || endPos != _endPos)
            {
                UpdateMousePosition(endPos);

                _startPos = startPos;
                _endPos = endPos;
            }
        }
    }
}
