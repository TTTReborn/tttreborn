using System.Collections.Generic;

using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationLabel : Label
    {
        private readonly static List<TranslationLabel> TranslationLabels = new();

        private string _translationKey;

        private object[] _translationParams;

        public TranslationLabel(string translationKey = null, string classname = null, params object[] args) : base()
        {
            SetTranslation(translationKey, args);
            AddClass(classname);

            TranslationLabels.Add(this);
        }

        public override void OnDeleted()
        {
            TranslationLabels.Remove(this);
        }

        public void SetTranslation(string translationKey, params object[] args)
        {
            _translationKey = translationKey;
            _translationParams = args;

            if (_translationKey is null)
            {
                return;
            }

            Text = TTTLanguage.GetActiveLanguage().GetFormattedTranslation(_translationKey, _translationParams);
        }

        public void UpdateTranslation(Language language)
        {
            if (_translationKey is null)
            {
                return;
            }

            Text = language.GetFormattedTranslation(_translationKey, _translationParams);
        }

        public static void UpdateLanguage(Language language)
        {
            foreach (TranslationLabel translationLabel in TranslationLabels)
            {
                translationLabel.UpdateTranslation(language);
            }
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TranslationLabelConstructor
    {
        public static TranslationLabel TranslationLabel(this PanelCreator self, string translationKey = null, string classname = null, params object[] args)
        {
            TranslationLabel translationLabel = new TranslationLabel(translationKey, classname, args);

            self.panel.AddChild(translationLabel);

            return translationLabel;
        }
    }
}
