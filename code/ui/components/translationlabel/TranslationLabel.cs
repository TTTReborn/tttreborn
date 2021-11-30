using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationLabel : Label, ITranslatable
    {
        [Property]
        public string Key
        {
            set
            {
                SetTranslation(new TranslationData(value));
            }
        }

        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
            }
        }

        private TranslationData _translationData = new();

        public TranslationLabel(TranslationData translationData, string classname = null) : base()
        {
            _translationData = translationData;

            SetTranslation(_translationData);
            AddClass("label");
            AddClass(classname);

            TTTLanguage.TranslationObjects.Add(this);
        }

        public override void OnDeleted()
        {
            TTTLanguage.TranslationObjects.Remove(this);

            base.OnDeleted();
        }

        public void SetTranslation(TranslationData translationData)
        {
            _translationData = translationData;
            base.Text = TTTLanguage.ActiveLanguage.GetFormattedTranslation(_translationData);
        }

        public void UpdateTranslation(Language language)
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
