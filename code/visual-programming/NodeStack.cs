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
            Instance?.Reset();

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
            // string defaultNodeStackJsonData = "{\"Nodes\":[{\"Id\":\"fcc2671c-7d11-4711-9ab0-58545f8a989d\",\"LibraryName\":\"stacknode_main\",\"NodeReference\":\"node_main\",\"ConnectionInputIds\":[],\"ConnectionOutputIds\":[null],\"Pos\":\"0,0\"}]}";
            string defaultNodeStackJsonData = "{\"Nodes\":[{\"Id\":\"fcc2671c-7d11-4711-9ab0-58545f8a989d\",\"LibraryName\":\"stacknode_main\",\"NodeReference\":\"node_main\",\"ConnectionInputIds\":[],\"ConnectionOutputIds\":[\"79e7f0db-ba09-4c7c-84d5-fda4b3032adc\"],\"Pos\":\"97,74\"},{\"Id\":\"79e7f0db-ba09-4c7c-84d5-fda4b3032adc\",\"LibraryName\":\"stacknode_percentage_selection\",\"NodeReference\":\"node_percentage_selection\",\"ConnectionInputIds\":[\"fcc2671c-7d11-4711-9ab0-58545f8a989d\"],\"ConnectionOutputIds\":[\"03c59f29-93be-4020-8f2a-cc3507ac16ba\",\"f1176a04-dd7b-4437-b518-916943eac7bf\"],\"Pos\":\"376,69\",\"PercentList\":[70,30]},{\"Id\":\"03c59f29-93be-4020-8f2a-cc3507ac16ba\",\"LibraryName\":\"stacknode_role_selection\",\"NodeReference\":\"node_role_selection\",\"ConnectionInputIds\":[\"79e7f0db-ba09-4c7c-84d5-fda4b3032adc\"],\"ConnectionOutputIds\":[\"aac6e985-7cfb-47fa-b7c0-c4f96ceb9241\"],\"Pos\":\"681,35\",\"SelectedRole\":\"role_innocent\"},{\"Id\":\"f1176a04-dd7b-4437-b518-916943eac7bf\",\"LibraryName\":\"stacknode_role_selection\",\"NodeReference\":\"node_role_selection\",\"ConnectionInputIds\":[\"79e7f0db-ba09-4c7c-84d5-fda4b3032adc\"],\"ConnectionOutputIds\":[null],\"Pos\":\"680,164\",\"SelectedRole\":\"role_traitor\"},{\"Id\":\"aac6e985-7cfb-47fa-b7c0-c4f96ceb9241\",\"LibraryName\":\"stacknode_playeramount_selection\",\"NodeReference\":\"node_playeramount_selection\",\"ConnectionInputIds\":[\"03c59f29-93be-4020-8f2a-cc3507ac16ba\"],\"ConnectionOutputIds\":[null,\"83ab4aba-b6e9-493c-89a6-afcb05d63658\"],\"Pos\":\"985,37\",\"PlayerAmountList\":[0,6]},{\"Id\":\"ce129a25-0748-48ff-a711-c3346939d7b7\",\"LibraryName\":\"stacknode_role_selection\",\"NodeReference\":\"node_role_selection\",\"ConnectionInputIds\":[\"83ab4aba-b6e9-493c-89a6-afcb05d63658\"],\"ConnectionOutputIds\":[null],\"Pos\":\"1338,64\",\"SelectedRole\":\"role_detective\"},{\"Id\":\"83ab4aba-b6e9-493c-89a6-afcb05d63658\",\"LibraryName\":\"stacknode_percentage_selection\",\"NodeReference\":\"node_percentage_selection\",\"ConnectionInputIds\":[\"aac6e985-7cfb-47fa-b7c0-c4f96ceb9241\"],\"ConnectionOutputIds\":[\"ce129a25-0748-48ff-a711-c3346939d7b7\",null],\"Pos\":\"989,189\",\"PercentList\":[17,83]}]}";

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
            Reset();

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

                jsonData = JsonSerializer.Serialize(jsonDict);
            }

            FileSystem.Data.WriteAllText(path, jsonData);

            Log.Debug($"Saved '{jsonData}' into '{path}'.");
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
