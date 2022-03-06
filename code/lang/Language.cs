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

        private readonly Dictionary<string, string> _langDict;

        public Language(string language, string json)
        {
            Dictionary<string, object> languageDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (languageDictionary.TryGetValue("__LANGUAGE__", out object dictJson))
            {
                Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(dictJson.ToString());

                if (!dict.TryGetValue("NAME", out string name) || string.IsNullOrEmpty(name))
                {
                    Log.Error($"Language '{language}' is missing '__LANGUAGE__.NAME' (language name)!");
                }

                if (!dict.TryGetValue("CODE", out string code) || string.IsNullOrEmpty(code))
                {
                    Log.Error($"Language '{language}' is missing '__LANGUAGE__.CODE' (language ISO (tag) code)!");
                }

                Data = new LanguageData()
                {
                    Name = name,
                    Code = code
                };
            }

            try {
                _langDict = TransformDictionary(languageDictionary);
            }
            catch (Exception e)
            {
                Log.Error($"Language parser registered error '{e.Message}' during processing the language '{language}': {e.StackTrace}");

                throw;
            }
        }

        private Dictionary<string, string> TransformDictionary(Dictionary<string, object> languageDictionary)
        {
            Dictionary<string, string> dictionary = new();

            foreach (KeyValuePair<string, object> translationEntry in languageDictionary)
            {
                try {
                    dictionary.Add(translationEntry.Key, JsonSerializer.Deserialize<string>((JsonElement) translationEntry.Value));
                }
                catch (Exception)
                {
                    Dictionary<string, string> transformedDictionary = TransformDictionary(JsonSerializer.Deserialize<Dictionary<string, object>>(((JsonElement) translationEntry.Value).GetRawText()));

                    foreach (KeyValuePair<string, string> transformedEntry in transformedDictionary)
                    {
                        dictionary.Add(translationEntry.Key + '.' + transformedEntry.Key, transformedEntry.Value);
                    }
                }
            }

            return dictionary;
        }

        public void AddTranslationString(string key, string value)
        {
            if (!_langDict.TryAdd(key, value))
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

        private string GetRawTranslation(TranslationData translationData)
        {
            if (translationData.Key.Length == 0)
            {
                return string.Empty;
            }

            return _langDict.GetValueOrDefault(translationData.Key);
        }
    }
}
