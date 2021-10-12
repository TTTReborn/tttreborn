namespace TTTReborn.UI.VisualProgramming
{
    [Node("main")]
    public class MainNode : Node
    {
        public MainNode(Sandbox.UI.Panel parent = null) : base(parent)
        {
            SetTitle("MainNode");

            NodeAllPlayersSetting settingPanel = AddSetting<NodeAllPlayersSetting>();
            settingPanel.ToggleInput(false);
        }
    }
}
