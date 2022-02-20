using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationLabel : Label, ITranslatable
    {
        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
            }
        }

        private TranslationData _translationData = new();

        public TranslationLabel()
        {
            TTTLanguage.Translatables.Add(this);
        }

        public TranslationLabel(TranslationData translationData, string classname = null) : base()
        {
            UpdateTranslation(translationData);
            AddClass(classname);

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
            base.Text = TTTLanguage.ActiveLanguage.GetFormattedTranslation(_translationData);
        }

        public void UpdateLanguage(Language language)
        {
            base.Text = language.GetFormattedTranslation(_translationData);
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TranslationLabelConstructor
    {
        public static TranslationLabel TranslationLabel(this PanelCreator self, TranslationData translationData, string classname = null)
        {
            TranslationLabel translationLabel = new(translationData, classname);

            self.panel.AddChild(translationLabel);

            return translationLabel;
        }
    }
}
