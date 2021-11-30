using System;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationButton : Button, ITranslatable
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

        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
            }
        }

        private readonly TranslationData _translationData = new();

        public TranslationButton() : base() { }

        public TranslationButton(TranslationData translationData, Action onClick = null, string classname = null) : base(translationData.Key, null, onClick)
        {
            _translationData = translationData;

            SetTranslation();
            AddClass(classname);

            TTTLanguage.TranslationObjects.Add(this);
        }

        public override void OnDeleted()
        {
            TTTLanguage.TranslationObjects.Remove(this);

            base.OnDeleted();
        }

        public void SetTranslation()
        {
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

    public static class TranslationButtonConstructor
    {
        public static TranslationButton TranslationButton(this PanelCreator self, TranslationData translationData, string classname = null, Action onClick = null)
        {
            TranslationButton translationButton = new(translationData, onClick, classname);

            self.panel.AddChild(translationButton);

            return translationButton;
        }
    }
}
