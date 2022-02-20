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

                if (arr == null)
                {
                    return true;
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

            bool errors = false;

            for (int i = 0, j = 0; i < stackNode.NextNodes.Count; i++, j++)
            {
                StackNode node = stackNode.NextNodes[i];

                try
                {
                    object data = null;

                    if (arr.Length > i)
                    {
                        while (j < arr.Length)
                        {
                            data = arr[j];

                            if (data != null)
                            {
                                break;
                            }

                            j++;
                        }
                    }

                    TestNode(node, data);
                }
                catch (Exception e)
                {
                    errors = true;

                    Log.Warning(e);
                }
            }

            return !errors;
        }

        public bool Evaluate(params object[] input)
        {
            return EvaluateNode(MainStackNode, input);
        }

        private bool EvaluateNode(StackNode stackNode, params object[] input)
        {
            object[] arr;

            try
            {
                arr = stackNode.Evaluate(input);

                if (arr == null)
                {
                    return true;
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

            bool errors = false;

            for (int i = 0, j = 0; i < stackNode.NextNodes.Count; i++, j++)
            {
                StackNode node = stackNode.NextNodes[i];

                try
                {
                    object data = null;

                    if (arr.Length > i)
                    {
                        while (j < arr.Length)
                        {
                            data = arr[j];

                            if (data != null)
                            {
                                break;
                            }

                            j++;
                        }
                    }

                    EvaluateNode(node, data);
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
            string defaultNodeStackJsonData = "{\"MainStackNode\":{\"LibraryName\":\"stacknode_main\",\"ConnectPositions\":[0],\"NodeReference\":\"node_main\",\"NextNodes\":[{\"LibraryName\":\"stacknode_percentage_selection\",\"ConnectPositions\":[0,0],\"NodeReference\":\"node_percentage_selection\",\"NextNodes\":[{\"LibraryName\":\"stacknode_role_selection\",\"ConnectPositions\":[-1],\"NodeReference\":\"node_role_selection\",\"NextNodes\":[],\"PosX\":1248,\"PosY\":202,\"SelectedRole\":\"role_traitor\"},{\"LibraryName\":\"stacknode_role_selection\",\"ConnectPositions\":[-1],\"NodeReference\":\"node_role_selection\",\"NextNodes\":[],\"PosX\":1246,\"PosY\":671,\"SelectedRole\":\"role_innocent\"}],\"PosX\":809,\"PosY\":366,\"PercentList\":[30,70]}],\"PosX\":404,\"PosY\":452}}";

            Save(defaultNodeStackJsonData);
            LoadDefaultFile(false);
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
                test = Instance.Test();
            }
            catch (Exception) {}

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

            if (MainStackNode == null && jsonData == null)
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

        public Dictionary<string, object> GetJsonData()
        {
            return new()
            {
                ["MainStackNode"] = MainStackNode.GetJsonData()
            };
        }
    }
}
