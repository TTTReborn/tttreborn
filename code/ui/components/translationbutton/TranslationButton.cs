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

        private readonly TranslationData _translationData = new();

        public TranslationButton() : base() { }

        public TranslationButton(TranslationData translationData, string icon = null, string classname = null, Action onClick = null) : base(translationData.Key, icon, onClick)
        {
            _translationData = translationData;

            SetTranslation();
            AddClass(classname);

            TTTLanguage.Translatables.Add(this);
        }

        public override void OnDeleted()
        {
            TTTLanguage.Translatables.Remove(this);

            base.OnDeleted();
        }

        public void SetTranslation()
        {
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

    public static class TranslationButtonConstructor
    {
        public static TranslationButton TranslationButton(this PanelCreator self, TranslationData translationData, string icon = null, string classname = null, Action onClick = null)
        {
            TranslationButton translationButton = new(translationData, icon, classname, onClick);

            self.panel.AddChild(translationButton);

            return translationButton;
        }
    }
}
