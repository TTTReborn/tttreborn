using System;
using System.Text.Json;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;

namespace TTTReborn.Settings
{
    public abstract partial class Settings
    {
        public SettingsLoadingError LoadingError = SettingsLoadingError.None;

        public string JsonType { get; set; } = "Settings";

        public Settings()
        {

        }
    }

    public partial class ServerSettings : Settings
    {
        public static ServerSettings Instance
        {
            get => SettingsManager.Instance as ServerSettings;
        }

        public ServerSettings() : base()
        {

        }
    }

    public partial class ClientSettings : Settings
    {
        public static ClientSettings Instance
        {
            get => SettingsManager.Instance as ClientSettings;
        }

        public ClientSettings() : base()
        {

        }
    }

    public enum SettingsLoadingError
    {
        None, // no error
        Empty, // null data
        NotExist, // file does not exist
        Invalid, // not a settings json
        Malicious, // could not be parsed
        InvalidSettingsType // wrong settings type
    }

    public partial class SettingsManager
    {
        public static Settings Instance
        {
            get => _instance;
            set
            {
                if (_instance == value)
                {
                    return;
                }

                _instance = value;

                Event.Run(TTTEvent.Settings.Change);
            }
        }
        private static Settings _instance;

        public static void Load()
        {
            if (Host.IsClient)
            {
                Instance = SettingFunctions.LoadSettings<ClientSettings>();
            }
            else
            {
                Instance = SettingFunctions.LoadSettings<ServerSettings>();
            }

            if (Instance.LoadingError != SettingsLoadingError.None)
            {
                Unload(); // overwrite settings if they got invalid

                if (Instance.LoadingError != SettingsLoadingError.NotExist)
                {
                    Log.Warning("Your TTT Reborn settings were overwritten (reset) due to an error in the file!");
                }
            }
        }

        public static void Unload()
        {
            if (Host.IsClient)
            {
                SettingFunctions.SaveSettings(ClientSettings.Instance);
            }
            else
            {
                SettingFunctions.SaveSettings(ServerSettings.Instance);
            }
        }
    }

    public partial class SettingFunctions
    {
        public const string SETTINGS_FILE_EXTENSION = ".settings.json";

        public static string GetJSON<T>(T settings, bool compressed = false) where T : Settings
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = !compressed
            };

            return JsonSerializer.Serialize(settings, options);
        }

        public static T GetSettings<T>(string json) where T : Settings
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static T LoadSettings<T>(string path = null, string fileName = "default") where T : Settings, new()
        {
            SettingsLoadingError settingsLoadingError = SettingsLoadingError.None;

            string settingsName = Utils.GetTypeNameByType(typeof(T));

            path ??= $"/settings/{settingsName.ToLower()}/";

            T settings = null;

            if (FileSystem.Data.FileExists(path + fileName + SETTINGS_FILE_EXTENSION))
            {
                try
                {
                    settings = GetSettings<T>(FileSystem.Data.ReadAllText(path + fileName + SETTINGS_FILE_EXTENSION));

                    if (settings is null)
                    {
                        settingsLoadingError = SettingsLoadingError.Empty;
                    }
                    else if (!settings.JsonType.Equals(settingsName))
                    {
                        settingsLoadingError = SettingsLoadingError.Invalid;

                        if (!string.IsNullOrEmpty(settings.JsonType))
                        {
                            settingsLoadingError = SettingsLoadingError.InvalidSettingsType;
                        }
                    }
                }
                catch (Exception)
                {
                    settingsLoadingError = SettingsLoadingError.Malicious;
                }
            }
            else
            {
                settingsLoadingError = SettingsLoadingError.NotExist;
            }

            if (settings is null)
            {
                settingsLoadingError = SettingsLoadingError.Empty;

                settings = new T();
            }

            settings.LoadingError = settingsLoadingError;

            return settings;
        }

        public static void SaveSettings<T>(T settings, string path = null, string fileName = "default") where T : Settings
        {
            if (!FileSystem.Data.DirectoryExists("settings"))
            {
                FileSystem.Data.CreateDirectory("settings");
            }

            if (settings is null)
            {
                return;
            }

            string settingsName = Utils.GetTypeNameByType(typeof(T));

            path ??= $"/settings/{settingsName.ToLower()}/";

            if (!FileSystem.Data.DirectoryExists(path))
            {
                FileSystem.Data.CreateDirectory(path);
            }

            settings.JsonType = settingsName;

            FileSystem.Data.WriteAllText(path + fileName + SETTINGS_FILE_EXTENSION, GetJSON(settings));
        }
    }
}
