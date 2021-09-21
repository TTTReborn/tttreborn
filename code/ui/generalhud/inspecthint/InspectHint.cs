using System;

using Sandbox;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class InspectHint : EntityHintPanel
    {
        public InspectHint(string translationKey)
        {
            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("text-color-info");
            AddClass("text-shadow");

            TranslationLabel label = Add.TranslationLabel(translationKey, String.Empty, new object[] { $"{Input.GetKeyWithBinding("+iv_use").ToUpper()}" });
            label.Style.Padding = 10;

            Enabled = false;
        }
    }
}
