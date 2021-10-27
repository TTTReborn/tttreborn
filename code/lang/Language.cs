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

        public string GetTranslation(string key, bool returnError = true)
        {
            object translation = GetRawTranslation(key);

            if (translation != null)
            {
                return translation.ToString();
            }

            if (TTTLanguage.Languages.TryGetValue(TTTLanguage.FALLBACK_LANGUAGE, out Language fallbackLanguage) && fallbackLanguage != this)
            {
                return fallbackLanguage.GetTranslation(key, returnError);
            }

            if (!returnError)
            {
                return key;
            }

            return $"[ERROR: Translation of '{key}' not found]";
        }

        public object GetRawTranslation(string key)
        {
            return _langDict.TryGetValue(key, out object translation) ? translation : null;
        }

        public string GetFormattedTranslation(string key, params object[] args)
        {
            return TryFormattedTranslation(key, true, args);
        }

        public string TryFormattedTranslation(string key, bool error = true, params object[] args)
        {
            string translation = GetTranslation(key, error);

            if (args == null)
            {
                return translation;
            }

            object[] data = new object[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is TranslationData translationKey)
                {
                    data[i] = TryFormattedTranslation(translationKey.Key, error, translationKey.Data);
                }
                else
                {
                    data[i] = args[i];
                }
            }

            return String.Format(translation, data);
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
