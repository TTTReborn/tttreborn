using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationCheckbox : Checkbox, ITranslatable
    {
        [Property]
        public string Key
        {
            set
            {
                _translationData.Key = value;
                SetTranslation();
            }
        }

        private readonly TranslationData _translationData = new();

        public TranslationCheckbox() : base() { }

        public TranslationCheckbox(TranslationData translationData) : base()
        {
            _translationData = translationData;

            SetTranslation();

            TTTLanguage.TranslationObjects.Add(this);
        }

        public void SetTranslation()
        {
            LabelText = TTTLanguage.ActiveLanguage.GetFormattedTranslation(_translationData);
        }

        public void UpdateTranslation(Language language)
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
