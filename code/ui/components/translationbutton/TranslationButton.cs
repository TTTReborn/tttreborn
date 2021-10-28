using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Events;
using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationButton : Button
    {
        private readonly static List<TranslationButton> _translationButtons = new();

        public string TranslationKey;
        public object[] TranslationParams;
        public bool IsTranslationDisabled = false;
        public bool IsTryTranslation;

        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;

                TranslationKey = null;
                TranslationParams = null;
            }
        }

        public TranslationButton(string translationKey = null, string icon = null, Action onClick = null, string classname = null, bool tryTranslation = false, params object[] args) : base(translationKey, icon, onClick)
        {
            IsTryTranslation = tryTranslation;

            SetTranslation(translationKey, args);
            AddClass(classname);

            _translationButtons.Add(this);
        }

        public override void OnDeleted()
        {
            _translationButtons.Remove(this);

            base.OnDeleted();
        }

        public void SetTranslation(string translationKey, params object[] args)
        {
            TranslationKey = translationKey;
            TranslationParams = args;

            if (TranslationKey == null)
            {
                return;
            }

            base.Text = TTTLanguage.ActiveLanguage.TryFormattedTranslation(TranslationKey, !IsTryTranslation, TranslationParams);
        }

        public void UpdateTranslation(Language language)
        {
            if (TranslationKey == null || IsTranslationDisabled)
            {
                return;
            }

            base.Text = language.TryFormattedTranslation(TranslationKey, !IsTryTranslation, TranslationParams);
        }

        public static void UpdateLanguage(Language language)
        {
            foreach (TranslationButton translationButton in _translationButtons)
            {
                translationButton.UpdateTranslation(language);
            }
        }

        [Event(TTTEvent.Settings.LanguageChange)]
        public static void OnLanguageChange(Language oldLanguage, Language newLanguage)
        {
            UI.TranslationButton.UpdateLanguage(newLanguage);
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TranslationButtonConstructor
    {
        public static TranslationButton TranslationButton(this PanelCreator self, string translationKey = null, string classname = null, Action onClick = null, params object[] args)
        {
            TranslationButton translationButton = new(translationKey, null, onClick, classname, false, args);

            self.panel.AddChild(translationButton);

            return translationButton;
        }

        public static TranslationButton TryTranslationButton(this PanelCreator self, string translationKey = null, string classname = null, Action onClick = null, params object[] args)
        {
            TranslationButton translationButton = new(translationKey, null, onClick, classname, true, args);

            self.panel.AddChild(translationButton);

            return translationButton;
        }
    }
}
