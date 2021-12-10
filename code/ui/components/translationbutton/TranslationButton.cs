using System;

using Sandbox;
using Sandbox.Html;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationButton : Button, ITranslatable
    {
        private TranslationData _translationData = new();

        public TranslationButton() : base() { }

        public TranslationButton(TranslationData translationData, string icon = null, string classname = null, Action onClick = null) : base(translationData.Key, icon, onClick)
        {
            SetTranslation(translationData);
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
                _translationData.Key = value;
                SetTranslation(_translationData);
                return;
            }
        }

        public void SetTranslation(TranslationData translationData)
        {
            _translationData = translationData;
            Text = TTTLanguage.ActiveLanguage.GetFormattedTranslation(_translationData);
        }

        public void UpdateLanguage(Language language)
        {
            Text = language.GetFormattedTranslation(_translationData);
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
