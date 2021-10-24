using System;
using System.Collections.Generic;
using System.Text.Json;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class VisualProgrammingWindow : Window
    {
        public static VisualProgrammingWindow Instance;

        public MainNode MainNode;
        public List<Node> Nodes = new();
        public NodeConnectionWire ActiveNodeConnectionWire;

        private NodeStack _nodeStack;

        public VisualProgrammingWindow(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Instance = this;

            StyleSheet.Load("/ui/visual-programming/VisualProgrammingWindow.scss");

            AddClass("fullscreen");

            Header.NavigationHeader.OnCreateWindowHeader = (header) =>
            {
                Sandbox.UI.Button saveButton = new("save_as", "", () => Save());
                saveButton.AddClass("save");

                header.AddChild(saveButton);

                Sandbox.UI.Button loadButton = new("folder_open", "", () => Load());
                loadButton.AddClass("load");

                header.AddChild(loadButton);

                Sandbox.UI.Button playButton = new("play_arrow", "", () => Build());
                playButton.AddClass("play");

                header.AddChild(playButton);
            };

            Header.NavigationHeader.Reload();

            MainNode = AddNode<MainNode>();
            MainNode.Display();

            AddNode<RoleSelectionNode>().Display();
            AddNode<RoleSelectionNode>().Display();
            AddNode<PercentageSelectionNode>().Display();

            _nodeStack = new NodeStack(); // TODO move to server later
        }

        public T AddNode<T>() where T : Node, new()
        {
            T node = new T();

            AddNode(node);

            return node;
        }

        public void AddNode(Node node)
        {
            Content.AddChild(node);
            Nodes.Add(node);
        }
    }
}
