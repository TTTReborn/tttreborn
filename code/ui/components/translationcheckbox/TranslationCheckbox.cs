using System;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationCheckbox : Checkbox
    {
        [Property]
        public string Key
        {
            set
            {
                SetTranslation(value, Array.Empty<object>());
            }
        }

        public TranslationCheckbox() : base() { }

        public void SetTranslation(string translationKey, params object[] args)
        {
            LabelText = TTTLanguage.ActiveLanguage.TryFormattedTranslation(translationKey, true, args);
        }
    }
}