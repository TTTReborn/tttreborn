using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class LabelHintPanel : EntityHintPanel
    {
        private readonly Sandbox.UI.Label _label;

        public LabelHintPanel(string hintText) : base()
        {
            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("text-color-info");
            AddClass("text-shadow");

            _label = Add.Label(hintText);
            _label.Style.Padding = 10;

            Enabled = false;
        }
    }
}
