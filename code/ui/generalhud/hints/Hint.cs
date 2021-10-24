using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class Hint : EntityHintPanel
    {
        private readonly Label _label;

        public Hint(string hintText)
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

        public override void UpdateHintPanel(string hintText)
        {
            if (string.IsNullOrEmpty(hintText))
            {
                return;
            }

            _label.Text = hintText;
        }
    }
}
