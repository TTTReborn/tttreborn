using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationCheckbox : Checkbox, ITranslatable
    {
        private TranslationData _translationData = new();

        public TranslationCheckbox()
        {
            TTTLanguage.Translatables.Add(this);
        }

        public TranslationCheckbox(TranslationData translationData)
        {
            UpdateTranslation(translationData);

            TTTLanguage.Translatables.Add(this);
        }

        public override void OnDeleted()
        {
            TTTLanguage.Translatables.Remove(this);

            base.OnDeleted();
        }

        public override void SetProperty(string name, string value)
        {
            base.SetProperty(name, value);

            if (name == "key")
            {
                UpdateTranslation(new TranslationData(value));

                return;
            }
        }

        public void UpdateTranslation(TranslationData translationData)
        {
            _translationData = translationData;
            LabelText = TTTLanguage.ActiveLanguage.GetFormattedTranslation(_translationData);
        }

        public void UpdateLanguage(Language language)
        {
            LabelText = language.GetFormattedTranslation(_translationData);
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TranslationCheckboxConstructor
    {
        public static TranslationCheckbox TranslationCheckbox(this PanelCreator self, TranslationData translationData)
        {
            TranslationCheckbox translationCheckbox = new(translationData);

            self.panel.AddChild(translationCheckbox);

            return translationCheckbox;
        }
    }
}
