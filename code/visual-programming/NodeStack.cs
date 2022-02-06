using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

namespace TTTReborn.VisualProgramming
{
    public partial class NodeStack
    {
        public const string VISUALPROGRAMMING_FILE_EXTENSION = ".vp.json";

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

        public bool Test(List<StackNode> stackNodesList)
        {
            // TODO check for integrity (no missing connections)

            // check for functionality (data set etc.)
            foreach (StackNode stackNode in stackNodesList)
            {
                if (stackNode.ConnectionInputIds.Length == 0 && !TestNode(stackNode)) // enter recursion
                {
                    return false;
                }
            }

            return true;
        }

        private bool TestNode(StackNode stackNode, object[] input = null)
        {
            try
            {
                object[] array = stackNode.Test(input);

                // TODO pass to connected nodes!
            }
            catch (Exception e)
            {
                if (e is NodeStackException)
                {
                    Log.Warning($"Error in node '{GetType()}' testing: ({e.Source}): {e.Message}\n{e.StackTrace}");

                    return false;
                }

                throw;
            }

            return true;
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
            string defaultNodeStackJsonData = "{\"Nodes\":[{\"Id\":\"fcc2671c-7d11-4711-9ab0-58545f8a989d\",\"LibraryName\":\"stacknode_main\",\"NodeReference\":\"node_main\",\"ConnectionInputIds\":[],\"ConnectionOutputIds\":[null],\"Pos\":\"0,0\"}]}";

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

        private static string GetSettingsPathByData(Utils.Realm realm) => Utils.GetSettingsFolderPath(realm, null, "visualprogramming/");

        private static string DefaultSettingsFile => $"default{VISUALPROGRAMMING_FILE_EXTENSION}";

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

            if (!Instance.Test(StackNodeList))
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
