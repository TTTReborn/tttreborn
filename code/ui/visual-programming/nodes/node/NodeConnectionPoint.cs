namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionPoint : Panel
    {
        public NodeConnectionWire ConnectionWire { get; set; }

        public Vector2 Position
        {
            get
            {
                Rect rect = Box.Rect;

                return new Vector2(rect.left + rect.width * 0.5f, rect.top + rect.height * 0.5f);
            }
        }

        public NodeConnectionPoint() : base()
        {
            AddClass("nodeconnectionpoint");
        }
    }
}
