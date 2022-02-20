using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationTextEntry : TextEntry, ITranslatable
    {
        private TranslationData _translationData = new();

        public TranslationTextEntry()
        {
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

            if (name == "placeholder_key")
            {
                UpdateTranslation(new TranslationData(value));

                return;
            }
        }

        public void UpdateTranslation(TranslationData translationData)
        {
            _translationData = translationData;
            Placeholder = TTTLanguage.ActiveLanguage.GetFormattedTranslation(_translationData);
        }

        public void UpdateLanguage(Language language)
        {
            Placeholder = language.GetFormattedTranslation(_translationData);
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TranslationTextEntryConstructor
    {
        public static TranslationTextEntry TranslationTextEntry(this PanelCreator self)
        {
            TranslationTextEntry translationTextEntry = new();

            self.panel.AddChild(translationTextEntry);

            return translationTextEntry;
        }
    }
}
