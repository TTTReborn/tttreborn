namespace TTTReborn.UI.VisualProgramming
{
    [Node("main")]
    public class MainNode : Node
    {
        public MainNode() : base()
        {
            SetTitle("MainNode");

            AddSetting<NodeAllPlayersSetting>().ToggleInput(false);
        }

        public override void Evaluate(params object[] input)
        {
            base.Evaluate(Globals.Utils.GetPlayers());
        }
    }
}
