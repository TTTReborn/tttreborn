using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class TranslationLabelHintPanel : EntityHintPanel
    {
        public readonly TranslationLabel TranslationLabel;

        public TranslationLabelHintPanel(string translationKey = null, params object[] args) : base()
        {
            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("text-color-info");
            AddClass("text-shadow");

            TranslationLabel = Add.TranslationLabel(translationKey, "hint", args);
            TranslationLabel.Style.Padding = 10;

            Enabled = false;
        }
    }
}
