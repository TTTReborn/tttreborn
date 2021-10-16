using Sandbox.UI.Construct;
using Sandbox.UI;

namespace TTTReborn.UI
{
    using System;

    public class UsableHint : EntityHintPanel
    {
        private readonly Label _label;

        public UsableHint(string hintText)
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

        public override void UpdateHintPanel(string currentHintText)
        {
            if (String.IsNullOrEmpty(currentHintText))
            {
                return;
            }

            _label.Text = currentHintText;
        }
    }
}
