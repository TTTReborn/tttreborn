using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class GlyphHint : EntityHintPanel
    {
        private readonly TranslationLabel _label;

        public GlyphHint(TranslationData translationData, params InputButton[] inputButtons) : base()
        {
            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("text-color-info");
            AddClass("text-shadow");

            for (int i = 0; i < inputButtons.Length; ++i)
            {
                AddChild(new BindingKeyImage(inputButtons[i]));

                // Don't show a + if it's the last binding in the list.
                if (i != inputButtons.Length - 1)
                {
                    Add.Label(" + ", "text-color-info");
                }
            }

            _label = Add.TranslationLabel(translationData);
            _label.Style.Padding = 10;

            this.Enabled(false);
        }

        public override void UpdateHintPanel(TranslationData translationData)
        {
            _label.UpdateTranslation(translationData);
        }
    }
}
