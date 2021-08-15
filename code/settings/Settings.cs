using System.Text.Json;

using Sandbox;

namespace TTTReborn.Settings
{
    public abstract partial class RealmSettings
    {
        public static RealmSettings Instance;

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

    public partial class SettingFunctions
    {
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

        public static void LoadSettings(string fileName = null)
        {
            if (Host.IsClient)
            {
                fileName ??= "client";

                if (FileSystem.Data.FileExists($"/settings/{fileName}.json"))
                {
                    GetRealmSettings<ClientSettings>(FileSystem.Data.ReadAllText($"/settings/{fileName}.json"));
                }

                if (ClientSettings.Instance is null)
                {
                    new ClientSettings();
                }
            }
            else
            {
                fileName ??= "server";

                if (FileSystem.Data.FileExists($"/settings/{fileName}.json"))
                {
                    GetRealmSettings<ServerSettings>(FileSystem.Data.ReadAllText($"/settings/{fileName}.json"));
                }

                if (ServerSettings.Instance is null)
                {
                    new ServerSettings();
                }
            }
        }

        public static void SaveSettings(string fileName = null)
        {
            if (!FileSystem.Data.DirectoryExists("settings"))
            {
                FileSystem.Data.CreateDirectory("settings");
            }

            if (Host.IsClient)
            {
                fileName ??= "client";

                if (ClientSettings.Instance is not null)
                {
                    FileSystem.Data.WriteAllText($"/settings/{fileName}.json", GetJSON<ClientSettings>(ClientSettings.Instance));
                }
            }
            else
            {
                fileName ??= "server";

                if (ServerSettings.Instance is not null)
                {
                    FileSystem.Data.WriteAllText($"/settings/{fileName}.json", GetJSON<ServerSettings>(ServerSettings.Instance));
                }
            }
        }
    }
}
