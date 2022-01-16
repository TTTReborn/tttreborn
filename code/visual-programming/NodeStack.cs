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

        private List<StackNode> StackNodeList { get; set; } = new();

        public NodeStack()
        {
            Instance = this;
        }

        public void Reset()
        {
            StackNodeList.Clear();
        }

        public bool Test(params object[] input)
        {
            foreach (StackNode stackNode in StackNodeList)
            {
                // TODO check / update input / process connected nodes?
                if (TestNode(stackNode, input) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private object[] TestNode(StackNode stackNode, params object[] input)
        {
            try
            {
                return stackNode.Test(input);
            }
            catch (Exception e)
            {
                if (e is NodeStackException)
                {
                    Log.Warning($"Error in node '{GetType()}' testing: ({e.Source}): {e.Message}\n{e.StackTrace}");

                    return null;
                }

                throw;
            }
        }

        public bool Evaluate(params object[] input)
        {
            // TODO // See Test()
            //return EvaluateNode(stackNode, input);

            return true;
        }

        private object[] EvaluateNode(StackNode stackNode, params object[] input)
        {
            try
            {
                return stackNode.Evaluate(input);
            }
            catch (Exception e)
            {
                if (e is NodeStackException)
                {
                    Log.Warning($"Error in node '{GetType()}' evaluation: ({e.Source}): {e.Message}\n{e.StackTrace}");

                    return null;
                }

                throw;
            }
        }

        public void Init()
        {
            // TODO
            // string defaultNodeStackJsonData = "{\"MainStackNode\":{\"LibraryName\":\"stacknode_main\",\"ConnectPositions\":[0],\"NodeReference\":\"node_main\",\"NextNodes\":[{\"LibraryName\":\"stacknode_percentage_selection\",\"ConnectPositions\":[0,0],\"NodeReference\":\"node_percentage_selection\",\"NextNodes\":[{\"LibraryName\":\"stacknode_role_selection\",\"ConnectPositions\":[-1],\"NodeReference\":\"node_role_selection\",\"NextNodes\":[],\"PosX\":1248,\"PosY\":202,\"SelectedRole\":\"role_traitor\"},{\"LibraryName\":\"stacknode_role_selection\",\"ConnectPositions\":[-1],\"NodeReference\":\"node_role_selection\",\"NextNodes\":[],\"PosX\":1246,\"PosY\":671,\"SelectedRole\":\"role_innocent\"}],\"PosX\":809,\"PosY\":366,\"PercentList\":[30,70]}],\"PosX\":404,\"PosY\":452}}";
            string defaultNodeStackJsonData = "{Nodes:[{\"LibraryName\":\"stacknode_main\",\"ConnectPositions\":[],\"NodeReference\":\"node_main\",\"NextNodes\":[],\"PosX\":404,\"PosY\":452}]}";

            Save(defaultNodeStackJsonData);
            LoadDefaultFile(false);
        }

        public void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("Nodes", out object jsonNodesData);

            if (jsonNodesData == null)
            {
                Log.Error("Malformed data in visual programming json");

                return;
            }

            List<object> jsonNodeList = JsonSerializer.Deserialize<List<object>>((JsonElement) jsonNodesData);

            foreach (object stackNodeJson in jsonNodeList)
            {
                if (stackNodeJson == null)
                {
                    continue;
                }

                Dictionary<string, object> saveStackNode = JsonSerializer.Deserialize<Dictionary<string, object>>(((JsonElement) stackNodeJson).GetRawText());

                StackNodeList.Add(StackNode.GetStackNodeFromJsonData<StackNode>(saveStackNode));
            }
        }

        private string GetSettingsPathByData(Utils.Realm realm) => Utils.GetSettingsFolderPath(realm, null, "visualprogramming/");

        private string DefaultSettingsFile => $"default{VISUALPROGRAMMING_FILE_EXTENSION}";

        public static void Load()
        {
            new NodeStack().LoadDefaultFile();
        }

        public void LoadDefaultFile(bool initial = true)
        {
            string settingsPath = GetSettingsPathByData(Utils.Realm.Server);

            Dictionary<string, object> jsonData = Player.TTTPlayer.LoadVisualProgramming(settingsPath, DefaultSettingsFile, Utils.Realm.Server);

            if (jsonData == null)
            {
                if (initial)
                {
                    Log.Warning($"VisualProgramming file '{settingsPath}{DefaultSettingsFile}' can't be loaded. Initializing new one...");

                    Init();
                }

                return;
            }

            LoadFromJsonData(jsonData);

            if (!Instance.Test())
            {
                Log.Warning($"VisualProgramming file '{settingsPath}{DefaultSettingsFile}' test failed. Initializing new one...");

                Init();
            }
        }

        public void Save(string jsonData = null)
        {
            if (!FileSystem.Data.DirectoryExists("settings"))
            {
                FileSystem.Data.CreateDirectory("settings");
            }

            if (StackNodeList.Count == 0 && jsonData == null)
            {
                return;
            }

            string settingsPath = GetSettingsPathByData(Utils.Realm.Server);

            if (!FileSystem.Data.DirectoryExists(settingsPath))
            {
                FileSystem.Data.CreateDirectory(settingsPath);
            }

            string path = $"{settingsPath}{DefaultSettingsFile}";

            FileSystem.Data.WriteAllText(path, jsonData ?? JsonSerializer.Serialize(GetJsonData()));
        }

        public List<object> GetJsonData()
        {
            List<object> jsonData = new();

            foreach (StackNode stackNode in StackNodeList)
            {
                jsonData.Add(stackNode.GetJsonData());
            }

            return jsonData;
        }
    }
}
