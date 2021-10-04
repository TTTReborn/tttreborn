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

        private static int _currentPacketHash = -1;
        private static int _packetCount;
        private static string[] _packetData;

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

                    if (settings == null)
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

            if (settings == null)
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

            if (settings == null)
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

        public static void SendSettingsToServer(ServerSettings settings)
        {
            string settingsJson = GetJSON(settings, true);
            int splitLength = 150;
            int splitCount = (int) MathF.Ceiling((float) settingsJson.Length / splitLength);

            for (int i = 0; i < splitCount; i++)
            {
                ServerSendPartialSettings(settingsJson.GetHashCode(), i, splitCount, settingsJson.Substring(splitLength * i, splitLength + Math.Min(0, settingsJson.Length - splitLength * (i + 1))));
            }
        }

        [ServerCmd]
        private static void ServerSendPartialSettings(int packetHash, int packetNum, int maxPackets, string partialSettings)
        {
            if (!ConsoleSystem.Caller?.HasPermission("serversettings") ?? true)
            {
                return;
            }

            ProceedPartialSettings(packetHash, packetNum, maxPackets, partialSettings);
        }

        private static void ProceedPartialSettings(int packetHash, int packetNum, int maxPackets, string partialSettings)
        {
            if (_currentPacketHash != packetHash)
            {
                _packetCount = 0;
                _packetData = new string[maxPackets];

                _currentPacketHash = packetHash;
            }

            if (_packetData[packetNum] != null && _packetData[packetNum].Equals(partialSettings))
            {
                return;
            }

            _packetData[packetNum] = partialSettings;
            _packetCount++;

            if (_packetCount == maxPackets)
            {
                _currentPacketHash = -1;

                ServerSettings serverSettings = SettingFunctions.GetSettings<ServerSettings>(string.Join("", _packetData));

                if (serverSettings == null)
                {
                    return;
                }

                SettingsManager.Instance = serverSettings;

                SettingFunctions.SaveSettings<ServerSettings>(ServerSettings.Instance);
            }
        }

        [ServerCmd]
        public static void RequestServerSettings()
        {
            if (!ConsoleSystem.Caller.HasPermission("serversettings"))
            {
                return;
            }

            ClientSendServerSettings(To.Single(ConsoleSystem.Caller), SettingFunctions.GetJSON<ServerSettings>(ServerSettings.Instance, true));
        }

        [ClientRpc]
        public static void ClientSendServerSettings(string serverSettingsJson)
        {
            ServerSettings serverSettings = SettingFunctions.GetSettings<ServerSettings>(serverSettingsJson);

            if (serverSettings == null)
            {
                return;
            }

            UI.Menu.Menu.Instance?.ProceedServerSettings(serverSettings);
        }
    }
}
