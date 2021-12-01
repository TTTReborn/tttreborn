using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Events;
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

        public TranslationData TranslationData;

        public TranslationLabel() : base()
        {
            AddClass("label");

            TTTLanguage.Translatables.Add(this);
        }

        public TranslationLabel(TranslationData translationData, string classname = null) : base()
        {
            SetTranslation(translationData);
            AddClass("label");
            AddClass(classname);

            TTTLanguage.Translatables.Add(this);
        }

        public override void OnDeleted()
        {
            TTTLanguage.Translatables.Remove(this);

            base.OnDeleted();
        }

        public void SetTranslation(TranslationData translationData)
        {
            TranslationData = translationData;

            base.Text = TTTLanguage.ActiveLanguage.GetFormattedTranslation(TranslationData);
        }

        public void UpdateLanguage(Language language)
        {
            base.Text = language.GetFormattedTranslation(TranslationData);
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
