namespace TTTReborn.UI.VisualProgramming
{
    [Node("conditional_placeholder")]
    public class ConditionalPlaceholderNode : Node
    {
        public ConditionalPlaceholderNode() : base()
        {
            SetTitle("Conditional Placeholder Node");

            NodeAllPlayersSetting settingPanel = AddSetting<NodeAllPlayersSetting>();
        }
    }
}
