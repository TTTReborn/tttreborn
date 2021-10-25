namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionWire : Panel
    {
        public NodeConnectionStartPoint StartPoint;
        public NodeConnectionEndPoint EndPoint;

        private Vector2 _startPos, _endPos;
        private NodeConnectionWirePart _startLine, _endLine, _midLine, _startMidLine, _endMidLine;

        private const float MIN_OVERLAP_WIDTH = 60f;
        private const float LINE_THICKNESS = 10f;

        public NodeConnectionWire(Sandbox.UI.Panel parent = null) : base(parent)
        {
            AddClass("nodeconnectionwire");

            _startLine = new(this);
            _startLine.AddClass("start");

            _startMidLine = new(this);
            _startMidLine.AddClass("startmid");

            _midLine = new(this);
            _midLine.AddClass("mid");

            _endMidLine = new(this);
            _endMidLine.AddClass("endmid");

            _endLine = new(this);
            _endLine.AddClass("end");

            Style.Position = Sandbox.UI.PositionMode.Absolute;
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
            return new NodeConnectionWire(Window.Instance.Content);
        }

        public void UpdateMousePosition(Vector2 vector2)
        {
            _startMidLine.Style.Display = Sandbox.UI.DisplayMode.None;
            _midLine.Style.Display = Sandbox.UI.DisplayMode.None;
            _endMidLine.Style.Display = Sandbox.UI.DisplayMode.None;

            Vector2 pos = StartPoint.Position;
            Vector2 delta = vector2 - pos;
            Vector2 finalPos = new Vector2(pos);
            Vector2 finalSize = new Vector2(delta);

            float halfLineThickness = LINE_THICKNESS * 0.5f;

            bool lessY = delta.y < 0f;

            if (lessY)
            {
                finalPos.y = vector2.y;
                finalSize.y = -delta.y;
            }

            if (delta.x < MIN_OVERLAP_WIDTH * 2f)
            {
                finalPos.x += delta.x - MIN_OVERLAP_WIDTH - halfLineThickness;
                finalSize.x = MIN_OVERLAP_WIDTH * 2f - finalSize.x + halfLineThickness;

                SetPanelMatrix(_startLine, new Vector2(finalSize.x - MIN_OVERLAP_WIDTH - halfLineThickness, (lessY ? finalSize.y : 0f) - halfLineThickness), new Vector2(MIN_OVERLAP_WIDTH, LINE_THICKNESS));
                SetPanelMatrix(_endLine, new Vector2(0f, (lessY ? 0f : finalSize.y) - halfLineThickness), new Vector2(MIN_OVERLAP_WIDTH, LINE_THICKNESS));

                SetPanelMatrix(_midLine, new Vector2(0f, finalSize.y * 0.5f - halfLineThickness), new Vector2(finalSize.x + halfLineThickness, LINE_THICKNESS));

                SetPanelMatrix(_startMidLine, new Vector2(finalSize.x - halfLineThickness, (lessY ? finalSize.y * 0.5f : 0f) - halfLineThickness), new Vector2(LINE_THICKNESS, finalSize.y * 0.5f + LINE_THICKNESS));
                SetPanelMatrix(_endMidLine, new Vector2(0f, (lessY ? 0f : finalSize.y * 0.5f) - halfLineThickness), new Vector2(LINE_THICKNESS, finalSize.y * 0.5f + halfLineThickness));
            }
            else
            {
                float halfWidth = finalSize.x * 0.5f;

                SetPanelMatrix(_startLine, new Vector2(0f, (lessY ? finalSize.y : 0f) - halfLineThickness), new Vector2(halfWidth, LINE_THICKNESS));
                SetPanelMatrix(_endLine, new Vector2(halfWidth, (lessY ? 0f : finalSize.y) - halfLineThickness), new Vector2(halfWidth, LINE_THICKNESS));

                SetPanelMatrix(_midLine, new Vector2(halfWidth - halfLineThickness, -halfLineThickness), new Vector2(LINE_THICKNESS, finalSize.y + LINE_THICKNESS));
            }

            SetPanelMatrix(this, finalPos, finalSize);

            _startMidLine.Style.Dirty();
            _midLine.Style.Dirty();
            _endMidLine.Style.Dirty();
        }

        public static void SetPanelMatrix(Sandbox.UI.Panel panel, Vector2 pos, Vector2 size)
        {
            panel.Style.Display = Sandbox.UI.DisplayMode.Flex;
            panel.Style.Left = Sandbox.UI.Length.Pixels(pos.x);
            panel.Style.Top = Sandbox.UI.Length.Pixels(pos.y);
            panel.Style.Width = Sandbox.UI.Length.Pixels(size.x);
            panel.Style.Height = Sandbox.UI.Length.Pixels(size.y);
            panel.Style.Dirty();
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

    public class NodeConnectionWirePart : Panel
    {
        public NodeConnectionWire NodeConnectionWire { get; set; }

        public NodeConnectionWirePart(NodeConnectionWire nodeConnectionWire) : base(nodeConnectionWire)
        {
            NodeConnectionWire = nodeConnectionWire;

            AddClass("nodeconnectionwirepart");
        }

        protected override void OnRightClick(Sandbox.UI.MousePanelEvent e)
        {
            base.OnRightClick(e);

            if (NodeConnectionWire.StartPoint.IsDragging)
            {
                return;
            }

            NodeConnectionWire.Delete(true);
        }
    }
}
