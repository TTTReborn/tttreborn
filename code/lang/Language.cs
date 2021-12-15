using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TTTReborn.Globalization
{
    public struct LanguageData
    {
        public string Name;
        public string Code;

        public LanguageData(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }

    public class Language
    {
        public readonly LanguageData Data;

        private readonly Dictionary<string, object> _langDict;

        public Language(string language, string json)
        {
            _langDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (_langDict.TryGetValue("__LANGUAGE__", out object dictJson))
            {
                Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(dictJson.ToString());

                if (!dict.TryGetValue("NAME", out string name) || String.IsNullOrEmpty(name))
                {
                    Log.Error($"Language '{language}' is missing '__LANGUAGE__.NAME' (language name)!");
                }

                if (!dict.TryGetValue("CODE", out string code) || String.IsNullOrEmpty(code))
                {
                    Log.Error($"Language '{language}' is missing '__LANGUAGE__.CODE' (language ISO (tag) code)!");
                }

                Data = new LanguageData()
                {
                    Name = name,
                    Code = code
                };
            }
        }

        public void AddTranslationString(string key, string translation)
        {
            if (!_langDict.TryAdd(key, translation))
            {
                Log.Warning($"Couldn't add translation string ('{key}') to '{Data.Name}'");
            }
        }

        public string GetFormattedTranslation(TranslationData translationData)
        {
            string translation = GetTranslation(translationData);

            if (translationData.Args == null || translationData.Args.Length == 0)
            {
                return translation;
            }

            object[] data = new object[translationData.Args.Length];

            for (int i = 0; i < translationData.Args.Length; i++)
            {
                if (translationData.Args[i] is TranslationData nestedTranslationData)
                {
                    data[i] = GetFormattedTranslation(nestedTranslationData);
                }
                else
                {
                    data[i] = translationData.Args[i];
                }
            }

            return string.Format(translation, data);
        }

        private string GetTranslation(TranslationData translationData)
        {
            object translation = GetRawTranslation(translationData);

            if (translation != null)
            {
                return translation.ToString();
            }

            if (Settings.SettingsManager.Instance.General.ReturnMissingKeys)
            {
                return $"[MISSING: Translation of '{translationData.Key}' not found]";
            }

            if (TTTLanguage.Languages.TryGetValue(TTTLanguage.FALLBACK_LANGUAGE, out Language fallbackLanguage) && fallbackLanguage != this)
            {
                return fallbackLanguage.GetTranslation(translationData);
            }

            return translationData.Key;
        }

        private object GetRawTranslation(TranslationData translationData)
        {
            if (translationData.Key.Length == 0)
            {
                return string.Empty;
            }

            return _langDict.GetValueOrDefault(translationData.Key);
        }
    }
}
