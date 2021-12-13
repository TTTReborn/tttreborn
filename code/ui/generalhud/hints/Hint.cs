using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class Hint : EntityHintPanel
    {
        private readonly TranslationLabel _label;

        public Hint(TranslationData translationData)
        {
            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("text-color-info");
            AddClass("text-shadow");

            _label = Add.TranslationLabel(translationData, string.Empty);
            _label.Style.Padding = 10;

            Enabled = false;
        }

        public override void UpdateHintPanel(TranslationData translationData)
        {
            _label.UpdateTranslation(translationData);
        }
    }
}
