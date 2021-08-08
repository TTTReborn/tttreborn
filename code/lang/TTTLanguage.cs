using System;
using System.IO;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Language
{
    public static class TTTLanguage
    {
        // TODO: On cvar change callback
        [ClientVar("ttt_language", Help = "Your current language", Saved = true)]
        public static string ActiveLanguage { get; set; }

        [ServerVar("ttt_server_language", Help = "Server's current language", Saved = true)]
        public static string ServerLanguage { get; set; }
        public const string FALLBACK_LANGUAGE = "English";

        public static Dictionary<string, TLanguage> Languages = new();

        private static void FallbackLanguage()
        {
            if (Host.IsClient)
            {
                ActiveLanguage = FALLBACK_LANGUAGE;
            }

            if (Host.IsServer)
            {
                ConsoleSystem.SetValue("ttt_server_language", FALLBACK_LANGUAGE);
            }
        }

        public static void LoadLanguages()
        {            
            foreach ( string file in FileSystem.Mounted.FindFile("/lang/packs/", "*.json", false) )
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string json = FileSystem.Mounted.ReadAllText($"/lang/packs/{file}");
                Languages.Add(name, new TLanguage(name, json));

                Log.Info($"Added TTT language pack: {name}.");
            }

            FallbackLanguage();
        }

        public static TLanguage GetActiveLanguage() => GetLanguageByName(Host.IsServer ? ServerLanguage : ActiveLanguage);

        public static TLanguage GetLanguageByName(string name)
        {
            if (!Languages.TryGetValue(name, out TLanguage lang))
            {
                Log.Warning($"Tried to get a language that doesn't exist: {name}.");

                if (Host.IsServer ? ServerLanguage == name : ActiveLanguage == name)
                {
                    FallbackLanguage();
                }
            }

            return lang;
        }

        public static void AddTranslationString(string language, string key, string translation)
        {
            GetLanguageByName(language).AddTranslationString(key, translation);
        }
    }
}
