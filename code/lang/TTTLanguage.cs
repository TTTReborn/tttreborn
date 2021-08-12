using System.Collections.Generic;
using System.IO;

using Sandbox;

namespace TTTReborn.Globalization
{
    public static class TTTLanguage
    {
        public const string FALLBACK_LANGUAGE = "en-US";

        public static Language ActiveLanguage
        {
            get => _activeLanguage;
            set
            {
                _activeLanguage = value ?? GetLanguageByName(FALLBACK_LANGUAGE);
            }
        }
        private static Language _activeLanguage;

        private static Language ServerLanguage
        {
            get => _serverLanguage;
            set
            {
                _serverLanguage = value ?? GetLanguageByName(FALLBACK_LANGUAGE);
            }
        }
        private static Language _serverLanguage;

        public static readonly Dictionary<string, Language> Languages = new();

        public static void LoadLanguages()
        {
            foreach (string file in FileSystem.Mounted.FindFile("/lang/packs/", "*.json", false))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string json = FileSystem.Mounted.ReadAllText($"/lang/packs/{file}");

                Language language = new Language(name, json);

                Languages.Add(language.Data.Code ?? name, language);

                Log.Info($"Added language pack: '{name}'.");
            }

            Language fallbackLanguage = GetLanguageByName(FALLBACK_LANGUAGE);

            ActiveLanguage = fallbackLanguage;
            ServerLanguage = fallbackLanguage;
        }

        public static Language GetActiveLanguage() => Host.IsServer ? ServerLanguage : ActiveLanguage;

        public static Language GetLanguageByName(string name)
        {
            if (!Languages.TryGetValue(name, out Language lang))
            {
                Log.Warning($"Tried to get a language that does not exist: '{name}'.");
            }

            return lang;
        }
    }
}
