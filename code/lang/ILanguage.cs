using System;
using System.Collections.Generic;
using Sandbox;

namespace TTTReborn.Language
{
    public interface ILanguage
    {
        // TODO: On cvar change callback
        [ClientVar("ttt_language", Help = "Your current language", Saved = true)]
        public static string ActiveLanguage { get; set; }

        [ServerVar("ttt_server_language", Help = "Server's current language")]
        public static string ServerLanguage { get; set; }
        public const string FALLBACK_LANGUAGE = "English";

        public static Dictionary<string, TLanguage> Languages = new();

        public static void LoadLanguages()
        {
            // Forgive me, father, for I have sinned
            Languages.Add("English", new TLanguage("English", "{ \"LanguageName\": \"English\", \"RoleName_Innocent\": \"Innocent\", \"RoleName_Traitor\": \"Traitor\", \"RoundState_Waiting\": \"Waiting\", \"RoundState_Preparing\": \"Preparing\", \"RoundState_In Progress\": \"In progress\", \"RoundState_Post\": \"Post round\", \"TeamName_Innocents\": \"Innocents\", \"TeamName_Traitors\": \"Traitors\", \"PostRound_Header\": \"{0} WIN!\", \"PostRound_Text\": \"Thanks for playing TTT Reborn, more updates and stats to come!\"}"));
            Languages.Add("Russian", new TLanguage("Russian", "{ \"LanguageName\": \"Русский\", \"RoleName_Innocent\": \"Невиновный\", \"RoleName_Traitor\": \"Предатель\", \"RoundState_Waiting\": \"Ожидание\", \"RoundState_Preparing\": \"Подготовка\", \"RoundState_In Progress\": \"В процессе\", \"RoundState_Post\": \"Конец\", \"TeamName_Innocents\": \"Невиновные\", \"TeamName_Traitors\": \"Предатели\", \"PostRound_Header\": \"{0} ВЫЙГРАЛИ!\", \"PostRound_Text\": \"Спасибо, что играете в TTT Reborn!\" }"));
            Languages.Add("French", new TLanguage("French", "{ \"LanguageName\": \"Français\", \"RoleName_Innocent\": \"Innocent\", \"RoleName_Traitor\": \"Traitre\", \"RoundState_Waiting\": \"En attente\", \"RoundState_Preparing\": \"Préparation\", \"RoundState_In Progress\": \"En cours\", \"RoundState_Post\": \"Terminé\", \"TeamName_Innocents\": \"INNOCENTS\", \"TeamName_Traitors\": \"TRAITRES\", \"PostRound_Header\": \"VICTOIRE DES {0}!\", \"PostRound_Text\": \"Merci d'avoir joué à TTT Reborn!\" }"));

            if ( ActiveLanguage is null )
                ActiveLanguage = FALLBACK_LANGUAGE;
        }

        public static TLanguage GetActiveLanguage() => GetLanguageByName(ActiveLanguage);
        public static TLanguage GetLanguageByName(string name)
        {
            TLanguage lang = null;
            Languages.TryGetValue(name, out lang);
            return lang;
        }
    }
}
