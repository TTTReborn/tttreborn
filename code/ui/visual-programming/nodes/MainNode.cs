namespace TTTReborn.UI.VisualProgramming
{
    [Node("main")]
    public class MainNode : Node
    {
        public MainNode() : base()
        {
            SetTitle("MainNode");

            NodeAllPlayersSetting settingPanel = AddSetting<NodeAllPlayersSetting>();
            settingPanel.ToggleInput(false);
        }
    }
}
