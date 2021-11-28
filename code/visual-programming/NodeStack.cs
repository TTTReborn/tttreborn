using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

namespace TTTReborn.VisualProgramming
{
    public partial class NodeStack
    {
        public static string VISUALPROGRAMMING_FILE_EXTENSION = ".vp.json";

        private StackNode MainStackNode { get; set; }

        public NodeStack()
        {

        }

        public void Reset()
        {
            MainStackNode = null;
        }

        public void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("MainStackNode", out object saveListJson);

            if (saveListJson == null)
            {
                return;
            }

            Dictionary<string, object> saveStackNode = JsonSerializer.Deserialize<Dictionary<string, object>>(((JsonElement) saveListJson).GetRawText());

            MainStackNode = StackNode.GetStackNodeFromJsonData<StackNode>(saveStackNode);
        }

        private string GetSettingsPathByData(Utils.Realm realm) => Utils.GetSettingsFolderPath(realm, null, "visualprogramming/");

        public void Load()
        {
            string settingsPath = GetSettingsPathByData(Utils.Realm.Server);
            string fileName = $"default{VISUALPROGRAMMING_FILE_EXTENSION}";

            Dictionary<string, object> jsonData = Player.TTTPlayer.LoadVisualProgramming(settingsPath, fileName, Utils.Realm.Server);

            if (jsonData == null)
            {
                Log.Error($"VisualProgramming file '{settingsPath}{fileName}{VISUALPROGRAMMING_FILE_EXTENSION}' can't be loaded.");

                return;
            }

            LoadFromJsonData(jsonData);
        }

        public void Save()
        {
            if (!FileSystem.Data.DirectoryExists("settings"))
            {
                FileSystem.Data.CreateDirectory("settings");
            }

            if (MainStackNode == null)
            {
                return;
            }

            string settingsPath = GetSettingsPathByData(Utils.Realm.Server);

            if (!FileSystem.Data.DirectoryExists(settingsPath))
            {
                FileSystem.Data.CreateDirectory(settingsPath);
            }

            string fileName = $"default{VISUALPROGRAMMING_FILE_EXTENSION}";
            string path = $"{settingsPath}{fileName}{VISUALPROGRAMMING_FILE_EXTENSION}";

            FileSystem.Data.WriteAllText(path, JsonSerializer.Serialize(GetJsonData()));
        }

        public Dictionary<string, object> GetJsonData()
        {
            return new()
            {
                ["MainStackNode"] = MainStackNode.GetJsonData()
            };
        }
    }
}
