using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class GlyphHint : EntityHintPanel
    {
        public readonly List<InputButton> InputButtons;

        private TranslationData _translationData;
        private TranslationLabel _label;

        public GlyphHint(TranslationData translationData, params InputButton[] inputButtons) : base()
        {
            _translationData = translationData;
            InputButtons = new(inputButtons);

            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("text-color-info");
            AddClass("text-shadow");

            CreateContent();

            this.Enabled(false);
        }

        private void CreateContent()
        {
            for (int i = 0; i < InputButtons.Count; ++i)
            {
                AddChild(new BindingKeyImage(InputButtons[i]));

                // Don't show a + if it's the last binding in the list.
                if (i != InputButtons.Count - 1)
                {
                    Add.Label(" + ", "text-color-info");
                }
            }

            _label = Add.TranslationLabel(_translationData);
            _label.Style.Padding = 10;
        }

        public override void UpdateHintPanel(TranslationData translationData)
        {
            _translationData = translationData;
            bool invalid = false;

            foreach (Panel panel in Children)
            {
                if (panel is BindingKeyImage bindingKeyImage)
                {
                    if (!InputButtons.Contains(bindingKeyImage.InputButton))
                    {
                        invalid = true;

                        break;
                    }
                }
            }

            if (!invalid)
            {
                int count = 0;

                foreach (InputButton inputButton in InputButtons)
                {
                    foreach (Panel panel in Children)
                    {
                        if (panel is BindingKeyImage bindingKeyImage && inputButton == bindingKeyImage.InputButton)
                        {
                            count++;

                            break;
                        }
                    }
                }

                invalid = count != InputButtons.Count;
            }

            if (invalid)
            {
                DeleteChildren(true);
                CreateContent();
            }
            else
            {
                _label.UpdateTranslation(translationData);
            }
        }
    }
}
