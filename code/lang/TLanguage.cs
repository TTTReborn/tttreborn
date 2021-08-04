using System;
using System.Collections.Generic;
using System.Text.Json;


namespace TTTReborn.Language
{
    public class TLanguage
    {
        private Dictionary<string, string> Strings { get; set; }
        public string LanguageName { get; set; }

        public TLanguage(string language, string json)
        {
            Strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            LanguageName = language;
        }

        public string GetTranslation(string key)
        {
            string translation = $"[ERROR: Translation of {key} not found]";
            Strings.TryGetValue(key, out translation);

            return translation;
        }

        public string GetFormatedTranslation(string key, params object?[] args)
        {
            string translation = GetTranslation(key);

            if (args is null)
                return translation;

            return String.Format(translation, args);
        }
    }
}
