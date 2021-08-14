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

        public static void LoadSettings()
        {
            if (Host.IsClient)
            {
                if (FileSystem.Data.FileExists("/settings/client.json"))
                {
                    GetRealmSettings<ClientSettings>(FileSystem.Data.ReadAllText("/settings/client.json"));
                }

                if (ClientSettings.Instance is null)
                {
                    new ClientSettings();
                }
            }
            else
            {
                if (FileSystem.Data.FileExists("/settings/server.json"))
                {
                    GetRealmSettings<ServerSettings>(FileSystem.Data.ReadAllText("/settings/server.json"));
                }

                if (ServerSettings.Instance is null)
                {
                    new ServerSettings();
                }
            }
        }

        public static void SaveSettings()
        {
            if (!FileSystem.Data.DirectoryExists("settings"))
            {
                FileSystem.Data.CreateDirectory("settings");
            }

            if (Host.IsClient)
            {
                if (ClientSettings.Instance is not null)
                {
                    FileSystem.Data.WriteAllText("/settings/client.json", GetJSON<ClientSettings>(ClientSettings.Instance));
                }
            }
            else
            {
                if (ServerSettings.Instance is not null)
                {
                    FileSystem.Data.WriteAllText("/settings/server.json", GetJSON<ServerSettings>(ServerSettings.Instance));
                }
            }
        }
    }
}
