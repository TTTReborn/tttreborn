using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;


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
            string translation;
            if (!Strings.TryGetValue(key, out translation))
                translation = $"[ERROR: Translation of {key} not found]";

            return translation;
        }

        public string GetFormatedTranslation(string key, params object?[] args)
        {
            string translation = GetTranslation(key);

            if (args is null)
                return translation;

            return String.Format(translation, args);
        }

        public void AddTranslationString(string key, string translation)
        {
            if (!Strings.TryAdd(key, translation))
            {
                Log.Warning($"Couldn't add translation string ({key}) to {LanguageName}");
            }
        }
    }
}
