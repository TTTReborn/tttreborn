using System;
using System.Text.Json;

using Sandbox;

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
        public static ServerSettings Instance;

        public ServerSettings() : base()
        {

        }
    }

    public partial class ClientSettings : Settings
    {
        public static ClientSettings Instance;

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

    public partial class SettingsLoader
    {
        public static void Load()
        {
            Settings settings = null;

            if (Host.IsClient)
            {
                ClientSettings.Instance = SettingFunctions.LoadSettings<ClientSettings>();
                settings = ClientSettings.Instance;
            }
            else
            {
                ServerSettings.Instance = SettingFunctions.LoadSettings<ServerSettings>();
                settings = ServerSettings.Instance;
            }

            // overwrite settings if they got invalid
            if (settings.LoadingError != SettingsLoadingError.None)
            {
                if (Host.IsClient)
                {
                    SettingFunctions.SaveSettings<ClientSettings>(ClientSettings.Instance);
                }
                else
                {
                    SettingFunctions.SaveSettings<ServerSettings>(ServerSettings.Instance);
                }

                if (settings.LoadingError != SettingsLoadingError.NotExist)
                {
                    Log.Warning("Your TTT Reborn settings were overwritten (reset) due to an error in the file!");
                }
            }
        }

        public static void Unload()
        {
            if (Host.IsClient)
            {
                SettingFunctions.SaveSettings<ClientSettings>(ClientSettings.Instance);
            }
            else
            {
                SettingFunctions.SaveSettings<ServerSettings>(ServerSettings.Instance);
            }
        }
    }

    public partial class SettingFunctions
    {
        public const string SETTINGS_FILE_EXTENSION = ".settings.json";

        public static string GetJSON<T>(T settings) where T : Settings
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize<T>(settings, options);
        }

        public static T GetSettings<T>(string json) where T : Settings
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static T LoadSettings<T>(string path = null, string fileName = "default") where T : Settings, new()
        {
            SettingsLoadingError settingsLoadingError = SettingsLoadingError.None;

            Type settingsType = typeof(T);
            string settingsName = settingsType.FullName.Replace(settingsType.Namespace, "").TrimStart('.');

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

            Type settingsType = typeof(T);
            string settingsName = settingsType.FullName.Replace(settingsType.Namespace, "").TrimStart('.');

            path ??= $"/settings/{settingsName.ToLower()}/";

            if (!FileSystem.Data.DirectoryExists(path))
            {
                FileSystem.Data.CreateDirectory(path);
            }

            settings.JsonType = settingsName;

            FileSystem.Data.WriteAllText(path + fileName + SETTINGS_FILE_EXTENSION, GetJSON<T>(settings));
        }
    }
}
