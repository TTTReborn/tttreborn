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

            foreach (StackNode stackNode in stackNodesList)
            {
                stackNode.PreparedInputData = null;
            }

            // check for functionality (data set etc.)
            foreach (StackNode stackNode in stackNodesList)
            {
                if (stackNode.ConnectionInputIds.Length == 0 && !TestNode(stackNode, 0))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TestNode(StackNode stackNode, int inputIndex, object input = null)
        {
            if (stackNode.ConnectionInputIds.Length > 0)
            {
                if (stackNode.PreparedInputData == null)
                {
                    int count = 0;

                    for (int i = 0; i < stackNode.ConnectionInputIds.Length; i++)
                    {
                        if (stackNode.ConnectionInputIds[i] != null)
                        {
                            count++;
                        }
                    }

                    stackNode.PreparedInputData = new object[count];
                }

                stackNode.PreparedInputData[inputIndex] = input;

                for (int i = 0; i < stackNode.PreparedInputData.Length; i++)
                {
                    if (stackNode.PreparedInputData[i] == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                stackNode.PreparedInputData = new object[]
                {
                    input
                };
            }

            bool successful = true;

            try
            {
                object[] array = stackNode.Test(stackNode.PreparedInputData);

                for (int o = 0; o < stackNode.ConnectionOutputIds.Length; o++)
                {
                    string id = stackNode.ConnectionOutputIds[o];

                    if (id == null)
                    {
                        continue;
                    }

                    StackNode idStackNode = StackNode.GetById(id);

                    if (idStackNode == null)
                    {
                        Log.Warning($"Error in testing NodeStack with node {stackNode.Id} ('{stackNode.LibraryName}')");

                        return false;
                    }

                    for (int i = 0; i < idStackNode.ConnectionInputIds.Length; i++)
                    {
                        string inputId = idStackNode.ConnectionInputIds[i];

                        if (inputId == stackNode.Id && !TestNode(idStackNode, i, array[o]))
                        {
                            successful = false;
                        }
                    }
                }
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

            return successful;
        }

        public bool Evaluate(object input = null)
        {
            foreach (StackNode stackNode in StackNodeList)
            {
                stackNode.PreparedInputData = null;
            }

            foreach (StackNode stackNode in StackNodeList)
            {
                if (stackNode.ConnectionInputIds.Length == 0 && !EvaluateNode(stackNode, 0, input))
                {
                    return false;
                }
            }

            return true;
        }

        private bool EvaluateNode(StackNode stackNode, int inputIndex, object input = null)
        {
            if (stackNode.ConnectionInputIds.Length > 0)
            {
                if (stackNode.PreparedInputData == null)
                {
                    int count = 0;

                    for (int i = 0; i < stackNode.ConnectionInputIds.Length; i++)
                    {
                        if (stackNode.ConnectionInputIds[i] != null)
                        {
                            count++;
                        }
                    }

                    stackNode.PreparedInputData = new object[count];
                }

                stackNode.PreparedInputData[inputIndex] = input;

                for (int i = 0; i < stackNode.PreparedInputData.Length; i++)
                {
                    if (stackNode.PreparedInputData[i] == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                stackNode.PreparedInputData = new object[]
                {
                    input
                };
            }

            bool successful = true;

            try
            {
                object[] array = stackNode.Evaluate(stackNode.PreparedInputData);

                for (int o = 0; o < stackNode.ConnectionOutputIds.Length; o++)
                {
                    string id = stackNode.ConnectionOutputIds[o];

                    if (id == null)
                    {
                        continue;
                    }

                    StackNode idStackNode = StackNode.GetById(id);

                    if (idStackNode == null)
                    {
                        Log.Warning($"Error in evaluating NodeStack with node {stackNode.Id} ('{stackNode.LibraryName}')");

                        return false;
                    }

                    for (int i = 0; i < idStackNode.ConnectionInputIds.Length; i++)
                    {
                        string inputId = idStackNode.ConnectionInputIds[i];

                        if (inputId == stackNode.Id && !EvaluateNode(idStackNode, i, array[o]))
                        {
                            successful = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is NodeStackException)
                {
                    Log.Warning($"Error in node '{GetType()}' evaluation: ({e.Source}): {e.Message}\n{e.StackTrace}");

                    return false;
                }

                throw;
            }

            return successful;
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

            bool test = false;

            try
            {
                test = Instance.Test(StackNodeList);
            }
            catch (Exception) { }

            if (!test)
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

            if (jsonData == null)
            {
                Dictionary<string, object> jsonDict = new();
                jsonDict.Add("Nodes", GetJsonData());

                FileSystem.Data.WriteAllText(path, JsonSerializer.Serialize(jsonDict));
            }
            else
            {
                FileSystem.Data.WriteAllText(path, jsonData);
            }
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
