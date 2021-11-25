// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

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
