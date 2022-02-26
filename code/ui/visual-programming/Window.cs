using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox.UI;

/*
  TODO
 - fix matrix issues
*/

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window : UI.Window
    {
        public static Window Instance;

        public MainNode MainNode;
        public List<Node> Nodes;
        public NodeConnectionWire ActiveNodeConnectionWire;
        public WindowSidebar Sidebar;
        public PanelContent Workspace;

        public Button BuildButton;

        private Window(Panel parent, string jsonData) : base(parent)
        {
            Instance = this;
            Nodes = new();

            StyleSheet.Load("/ui/visual-programming/Window.scss");

            AddClass("fullscreen");
            AddClass("visualprogramming");

            Header.NavigationHeader.OnCreateWindowHeader = (header) =>
            {
                Button saveButton = new("", "save", () => Save());
                saveButton.AddClass("save");

                header.AddChild(saveButton);

                Button loadButton = new("", "folder_open", () => Load());
                loadButton.AddClass("load");

                header.AddChild(loadButton);

                BuildButton = new("", "play_arrow", () => Build());
                BuildButton.AddClass("play");

                header.AddChild(BuildButton);

                Button resetButton = new("", "delete", () => Reset());
                resetButton.AddClass("reset");

                header.AddChild(resetButton);
            };

            Header.NavigationHeader.Reload();

            Content.SetPanelContent((panelContent) =>
            {
                Workspace = new(panelContent);
                Sidebar = new(panelContent);

                LoadNodesFromStackJson(jsonData);

                foreach (Node node in Nodes)
                {
                    if (node is MainNode mainNode)
                    {
                        MainNode = mainNode;

                        break;
                    }
                }

                if (MainNode == null)
                {
                    MainNode = AddNode<MainNode>();
                    MainNode.Display();

                    Log.Warning("Missing main node in default visual programming stack");
                }
            });
        }

        public static Window Init(Panel parent, string jsonData)
        {
            if (Instance != null)
            {
                foreach (Node node in Instance.Nodes)
                {
                    node.Delete(true);
                }

                Instance.Nodes.Clear();
                Instance.Delete(true);
            }

            return new Window(parent, jsonData);
        }

        public T AddNode<T>() where T : Node, new()
        {
            T node = new();

            AddNode(node);

            return node;
        }

        public void AddNode(Node node)
        {
            Workspace.AddChild(node);
            Nodes.Add(node);
        }

        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
        }

        private void LoadNodesFromStackJson(string jsonData)
        {
            jsonData = jsonData.Replace("LibraryName", "StackNodeName").Replace("NodeReference", "LibraryName");

            List<object> jsonDataList = JsonSerializer.Deserialize<List<object>>(jsonData);

            foreach (object stackNode in jsonDataList)
            {
                Dictionary<string, object> saveStackNode = JsonSerializer.Deserialize<Dictionary<string, object>>(((JsonElement) stackNode).GetRawText());

                if (!saveStackNode.TryGetValue("LibraryName", out object libraryName))
                {
                    continue;
                }

                Type type = Utils.GetTypeByLibraryName<Node>(libraryName.ToString());

                if (type == null)
                {
                    continue;
                }

                Node node = Utils.GetObjectByType<Node>(type);

                if (node == null)
                {
                    continue;
                }

                node.LoadFromJsonData(saveStackNode);
                AddNode(node);
            }

            foreach (Node node in Nodes)
            {
                if (node is MainNode mainNode)
                {
                    MainNode = mainNode;
                }

                // connect nodes
                foreach (string id in node.ConnectionInputIds)
                {
                    if (id == null)
                    {
                        continue;
                    }

                    Node idNode = Node.GetById(id);

                    if (idNode == null)
                    {
                        continue;
                    }

                    idNode.ConnectWithNode(node);
                }

                node.Display();
            }

            Log.Debug($"Loaded: '{jsonData}'");
        }

        private Dictionary<string, object> GetStackNodesJsonDictionary()
        {
            Dictionary<string, object> jsonDict = new();

            // TODO add workspace settings to jsonDict as well

            List<Dictionary<string, object>> saveList = new();

            foreach (Node node in Nodes)
            {
                node.Prepare();

                saveList.Add(node.StackNode.GetJsonData());
            }

            jsonDict.Add("Nodes", saveList);

            return jsonDict;
        }
    }
}
