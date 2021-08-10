using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

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

                if (!dict.TryGetValue("CODE", out string code))
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

        public object GetRawTranslation(string key)
        {
            if (_langDict.TryGetValue(key, out object translation))
            {
                return translation;
            }

            return null;
        }

        public string GetTranslation(string key)
        {
            object translation = GetRawTranslation(key);

            if (translation is not null)
            {
                return translation.ToString();
            }
            else if (TTTLanguage.Languages.TryGetValue(TTTLanguage.FALLBACK_LANGUAGE, out Language fallbackLanguage) && fallbackLanguage != this)
            {
                return fallbackLanguage.GetTranslation(key);
            }

            return $"[ERROR: Translation of '{key}' not found]";
        }

        public string GetFormatedTranslation(string key, params object[] args)
        {
            string translation = GetTranslation(key);

            if (args is null)
            {
                return translation;
            }

            return String.Format(translation, args);
        }

        public void AddTranslationString(string key, string translation)
        {
            if (!_langDict.TryAdd(key, translation))
            {
                Log.Warning($"Couldn't add translation string ('{key}') to '{Data.Name}'");
            }
        }
    }
}
