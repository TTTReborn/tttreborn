using System.Collections.Generic;

using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationLabel : Label
    {
        private readonly static List<TranslationLabel> _translationLabels = new();

        private string _translationKey;

        private object[] _translationParams;

        public TranslationLabel(string translationKey = null, string classname = null, params object[] args) : base()
        {
            SetTranslation(translationKey, args);
            AddClass(classname);

            _translationLabels.Add(this);
        }

        public override void OnDeleted()
        {
            _translationLabels.Remove(this);

            base.OnDeleted();
        }

        public void SetTranslation(string translationKey, params object[] args)
        {
            _translationKey = translationKey;
            _translationParams = args;

            if (_translationKey == null)
            {
                return;
            }

            Text = TTTLanguage.ActiveLanguage.GetFormattedTranslation(_translationKey, _translationParams);
        }

        public void UpdateTranslation(Language language)
        {
            if (_translationKey == null)
            {
                return;
            }

            Text = language.GetFormattedTranslation(_translationKey, _translationParams);
        }

        public static void UpdateLanguage(Language language)
        {
            foreach (TranslationLabel translationLabel in _translationLabels)
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
