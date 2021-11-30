// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System.Collections.Generic;
using System.Text.Json;

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

        public Sandbox.UI.Button BuildButton;

        public Window(Sandbox.UI.Panel parent, string jsonData) : base(parent)
        {
            Instance = this;

            StyleSheet.Load("/ui/visual-programming/Window.scss");

            AddClass("fullscreen");
            AddClass("visualprogramming");

            Header.NavigationHeader.OnCreateWindowHeader = (header) =>
            {
                Sandbox.UI.Button saveButton = new("save_as", "", () => Save());
                saveButton.AddClass("save");

                header.AddChild(saveButton);

                Sandbox.UI.Button loadButton = new("folder_open", "", () => Load());
                loadButton.AddClass("load");

                header.AddChild(loadButton);

                BuildButton = new("play_arrow", "", () => Build());
                BuildButton.AddClass("play");

                header.AddChild(BuildButton);

                Sandbox.UI.Button resetButton = new("delete", "", () => Reset());
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
