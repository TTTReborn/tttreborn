using System;
using System.Text.Json;

using Sandbox;

namespace TTTReborn.Settings
{
    public abstract partial class RealmSettings
    {
        public static RealmSettings Instance;

        public SettingsLoadingError LoadingError = SettingsLoadingError.None;

        public string JsonType { get; set; } = "RealmSettings";

        public RealmSettings()
        {

        }
    }

    public partial class ServerSettings : RealmSettings
    {
        public new static ServerSettings Instance;

        public ServerSettings() : base()
        {

        }
    }

    public partial class ClientSettings : RealmSettings
    {
        public new static ClientSettings Instance;

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
        WrongRealm // wrong realm
    }

    public partial class SettingsLoader
    {
        public static void Load()
        {
            RealmSettings settings = null;

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

        public static string GetJSON<T>(T realmSettings) where T : RealmSettings
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize<T>(realmSettings, options);
        }

        public static T GetRealmSettings<T>(string json) where T : RealmSettings
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static T LoadSettings<T>(string path = null, string fileName = "default") where T : RealmSettings, new()
        {
            SettingsLoadingError settingsLoadingError = SettingsLoadingError.None;

            Type realmType = typeof(T);
            string realm = realmType.FullName.Replace(realmType.Namespace, "").TrimStart('.');

            path ??= $"/settings/{realm.ToLower()}/";

            T settings = null;

            if (FileSystem.Data.FileExists(path + fileName + SETTINGS_FILE_EXTENSION))
            {
                try
                {
                    settings = GetRealmSettings<T>(FileSystem.Data.ReadAllText(path + fileName + SETTINGS_FILE_EXTENSION));

                    if (settings is null)
                    {
                        settingsLoadingError = SettingsLoadingError.Empty;
                    }
                    else if (!settings.JsonType.Equals(realm))
                    {
                        settingsLoadingError = SettingsLoadingError.Invalid;

                        if (!string.IsNullOrEmpty(settings.JsonType))
                        {
                            settingsLoadingError = SettingsLoadingError.WrongRealm;
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

        public static void SaveSettings<T>(T settings, string path = null, string fileName = "default") where T : RealmSettings
        {
            if (!FileSystem.Data.DirectoryExists("settings"))
            {
                FileSystem.Data.CreateDirectory("settings");
            }

            if (settings is null)
            {
                return;
            }

            Type realmType = typeof(T);
            string realm = realmType.FullName.Replace(realmType.Namespace, "").TrimStart('.');

            path ??= $"/settings/{realm.ToLower()}/";

            if (!FileSystem.Data.DirectoryExists(path))
            {
                FileSystem.Data.CreateDirectory(path);
            }

            settings.JsonType = realm;

            FileSystem.Data.WriteAllText(path + fileName + SETTINGS_FILE_EXTENSION, GetJSON<T>(settings));
        }
    }
}
