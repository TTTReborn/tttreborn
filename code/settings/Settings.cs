using System;
using System.Text.Json;

using Sandbox;

namespace TTTReborn.Settings
{
    public abstract partial class RealmSettings
    {
        public static RealmSettings Instance;

        public string JsonType { get; set; } = "RealmSettings";

        public RealmSettings()
        {
            Instance = this;
        }
    }

    public partial class ServerSettings : RealmSettings
    {
        public new static ServerSettings Instance;

        public ServerSettings() : base()
        {
            Instance = this;
        }
    }

    public partial class ClientSettings : RealmSettings
    {
        public new static ClientSettings Instance;

        public ClientSettings() : base()
        {
            Instance = this;
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

        public static SettingsLoadingError LoadSettings(string filePath = null)
        {
            SettingsLoadingError settingsLoadingError = SettingsLoadingError.None;

            if (Host.IsClient)
            {
                filePath ??= "/settings/client";

                if (FileSystem.Data.FileExists($"{filePath}{SETTINGS_FILE_EXTENSION}"))
                {
                    try
                    {
                        ClientSettings settings = GetRealmSettings<ClientSettings>(FileSystem.Data.ReadAllText($"{filePath}{SETTINGS_FILE_EXTENSION}"));

                        if (settings is null)
                        {
                            settingsLoadingError = SettingsLoadingError.Empty;
                        }
                        else if (!settings.JsonType.Equals("ClientSettings"))
                        {
                            settingsLoadingError = SettingsLoadingError.Invalid;

                            if (settings.JsonType.Equals("ServerSettings"))
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

                if (ClientSettings.Instance is null)
                {
                    settingsLoadingError = SettingsLoadingError.Empty;

                    new ClientSettings();
                }
            }
            else
            {
                filePath ??= "/settings/server";

                if (FileSystem.Data.FileExists($"{filePath}{SETTINGS_FILE_EXTENSION}"))
                {
                    try
                    {
                        ServerSettings settings = GetRealmSettings<ServerSettings>(FileSystem.Data.ReadAllText($"{filePath}{SETTINGS_FILE_EXTENSION}"));

                        if (settings is null)
                        {
                            settingsLoadingError = SettingsLoadingError.Empty;
                        }
                        else if (!settings.JsonType.Equals("ServerSettings"))
                        {
                            settingsLoadingError = SettingsLoadingError.Invalid;

                            if (settings.JsonType.Equals("ClientSettings"))
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

                if (ServerSettings.Instance is null)
                {
                    settingsLoadingError = SettingsLoadingError.Empty;

                    new ServerSettings();
                }
            }

            return settingsLoadingError;
        }

        public static void SaveSettings(string filePath = null)
        {
            if (!FileSystem.Data.DirectoryExists("settings"))
            {
                FileSystem.Data.CreateDirectory("settings");
            }

            if (Host.IsClient)
            {
                filePath ??= "/settings/client";

                if (ClientSettings.Instance is not null)
                {
                    ClientSettings.Instance.JsonType = "ClientSettings";

                    FileSystem.Data.WriteAllText($"{filePath}{SETTINGS_FILE_EXTENSION}", GetJSON<ClientSettings>(ClientSettings.Instance));
                }
            }
            else
            {
                filePath ??= "/settings/server";

                if (ServerSettings.Instance is not null)
                {
                    ServerSettings.Instance.JsonType = "ServerSettings";

                    FileSystem.Data.WriteAllText($"{filePath}{SETTINGS_FILE_EXTENSION}", GetJSON<ServerSettings>(ServerSettings.Instance));
                }
            }
        }
    }
}
