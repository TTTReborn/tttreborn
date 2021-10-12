namespace TTTReborn.UI.VisualProgramming
{
    public class NodeSettingPanel : Panel
    {
        public NodeConnectionPanel Input { get; set; }
        public NodeConnectionPanel Output { get; set; }
        public PanelContent Content { get; set; }

        public NodeSettingPanel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Input = new(this);
            Content = new(this);
            Output = new(this);
        }
    }
}
