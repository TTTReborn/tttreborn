using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Events;
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
                TranslationKey = value;
                SetTranslation(value, Array.Empty<object>());
            }
        }

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

        public TranslationButton() : base() { }

        public TranslationButton(string translationKey = null, string icon = null, Action onClick = null, string classname = null, bool tryTranslation = false, params object[] args) : base(translationKey, icon, onClick)
        {
            IsTryTranslation = tryTranslation;

            SetTranslation(translationKey, args);
            AddClass(classname);

            TTTLanguage.TranslationObjects.Add(this);
        }

        public override void OnDeleted()
        {
            TTTLanguage.TranslationObjects.Remove(this);

            base.OnDeleted();
        }

        public void SetTranslation(string translationKey, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(translationKey))
            {
                translationKey = null;
            }

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
