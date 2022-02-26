using System;

using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationButton : Button, ITranslatable
    {
        private TranslationData _translationData = new();

        public TranslationButton()
        {
            TTTLanguage.Translatables.Add(this);
        }

        public TranslationButton(TranslationData translationData, string icon = null, string classname = null, Action onClick = null) : base(translationData.Key, icon, onClick)
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
            SetText(TTTLanguage.ActiveLanguage.GetFormattedTranslation(_translationData));
        }

        public void UpdateLanguage(Language language)
        {
            SetText(language.GetFormattedTranslation(_translationData));
        }

        private new void SetText(string value)
        {
            Text = value;
            SetClass("has-label", !string.IsNullOrEmpty(Text));
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
