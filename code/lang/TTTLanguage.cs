using System.Collections.Generic;
using System.IO;

using Sandbox;

namespace TTTReborn.Settings
{
    using Globalization;

    public partial class ServerSettings
    {
        public string Language
        {
            get => _language ?? TTTLanguage.ServerLanguage.Data.Code;
            set
            {
                if (_language == value)
                {
                    return;
                }

                _language = value;

                TTTLanguage.ServerLanguage = TTTLanguage.GetLanguageByCode(_language);
            }
        }
        private string _language;
    }

    public partial class ClientSettings
    {
        public string Language
        {
            get => _language ?? TTTLanguage.ActiveLanguage.Data.Code;
            set
            {
                if (_language == value)
                {
                    return;
                }

                _language = value;

                Language language = TTTLanguage.GetLanguageByCode(_language);

                TTTLanguage.ActiveLanguage = language;

                UI.TranslationLabel.UpdateLanguage(language);
            }
        }
        private string _language;
    }
}

namespace TTTReborn.Globalization
{
    public static class TTTLanguage
    {
        public static readonly Dictionary<string, Language> Languages = new();

        public const string FALLBACK_LANGUAGE = "en-US";

        public static Language ActiveLanguage
        {
            get => _activeLanguage;
            set
            {
                _activeLanguage = value ?? GetLanguageByCode(FALLBACK_LANGUAGE);
            }
        }
        private static Language _activeLanguage;

        public static Language ServerLanguage
        {
            get => _serverLanguage;
            set
            {
                _serverLanguage = value ?? GetLanguageByCode(FALLBACK_LANGUAGE);
            }
        }
        private static Language _serverLanguage;

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

            Language fallbackLanguage = GetLanguageByCode(FALLBACK_LANGUAGE);

            ActiveLanguage = fallbackLanguage;
            ServerLanguage = fallbackLanguage;
        }

        public static Language GetActiveLanguage() => Host.IsServer ? ServerLanguage : ActiveLanguage;

        public static Language GetLanguageByCode(string name)
        {
            if (!Languages.TryGetValue(name, out Language lang))
            {
                Log.Warning($"Tried to get a language that does not exist: '{name}'.");
            }

            return lang;
        }
    }
}
