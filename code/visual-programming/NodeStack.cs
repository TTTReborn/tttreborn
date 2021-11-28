using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

namespace TTTReborn.VisualProgramming
{
    public partial class NodeStack
    {
        public static string VISUALPROGRAMMING_FILE_EXTENSION = ".vp.json";

        public static NodeStack Instance;

        private StackNode MainStackNode { get; set; }

        public NodeStack()
        {
            Instance = this;
        }

        public void Reset()
        {
            MainStackNode = null;
        }

        public bool Test(params object[] input)
        {
            return TestNode(MainStackNode, input);
        }

        private bool TestNode(StackNode stackNode, params object[] input)
        {
            object[] arr;

            try
            {
                arr = stackNode.Test(input);
            }
            catch (Exception e)
            {
                if (e is NodeStackException)
                {
                    Log.Warning($"Error in node '{GetType()}': ({e.Source}): {e.Message}\n{e.StackTrace}");

                    return false;
                }

                throw;
            }

            bool errors = false;

            for (int i = 0; i < stackNode.NextNodes.Count; i++)
            {
                StackNode node = stackNode.NextNodes[i];

                try
                {
                    TestNode(node, arr.Length > i ? arr[i] : null);
                }
                catch (Exception e)
                {
                    errors = true;

                    Log.Warning(e);
                }
            }

            return !errors;
        }

        public void Init()
        {
            // TODO create default Stack with useful settings

            MainStackNode = new AllPlayersStackNode();
            MainStackNode.NodeReference = Utils.GetLibraryName(typeof(UI.VisualProgramming.MainNode));

            Save();
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

        private string DefaultSettingsFile => $"default{VISUALPROGRAMMING_FILE_EXTENSION}";

        public static void Load()
        {
            new NodeStack().LoadDefaultFile();
        }

        public void LoadDefaultFile()
        {
            string settingsPath = GetSettingsPathByData(Utils.Realm.Server);

            Dictionary<string, object> jsonData = Player.TTTPlayer.LoadVisualProgramming(settingsPath, DefaultSettingsFile, Utils.Realm.Server);

            if (jsonData == null)
            {
                Log.Warning($"VisualProgramming file '{settingsPath}{DefaultSettingsFile}' can't be loaded. Initializing new one...");

                Init();

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

            string path = $"{settingsPath}{DefaultSettingsFile}";

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
