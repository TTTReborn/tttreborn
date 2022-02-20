using System.Collections.Generic;
using System.Text.Json;

using Sandbox.UI;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window : UI.Window
    {
        public static Window Instance;

        public MainNode MainNode;
        public List<Node> Nodes = new();
        public NodeConnectionWire ActiveNodeConnectionWire;
        public WindowSidebar Sidebar;
        public PanelContent Workspace;

        public Button BuildButton;

        public Window(Panel parent, string jsonData) : base(parent)
        {
            Instance = this;

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

                if (MainNode == null)
                {
                    MainNode = AddNode<MainNode>();
                    MainNode.Display();

                    Log.Warning("Missing main node in default visual programming stack");
                }
            });
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

            Dictionary<string, object> jsonDataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);

            jsonDataDict.TryGetValue("MainStackNode", out object mainStackNode);

            if (mainStackNode == null)
            {
                return;
            }

            Dictionary<string, object> saveStackNode = JsonSerializer.Deserialize<Dictionary<string, object>>(((JsonElement) mainStackNode).GetRawText());

            MainNode = AddNode<MainNode>();
            MainNode.LoadFromJsonData(saveStackNode);

            foreach (Node node in Nodes)
            {
                node.Display();
            }

            Log.Debug($"Loaded: '{jsonData}'");
        }
    }
}
