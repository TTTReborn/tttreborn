using System;
using System.Collections.Generic;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    public class VisualProgrammingWindow : Window
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

        public void Build()
        {
            _nodeStack.Reset();

            bool hasError = false;

            foreach (Node node in Nodes)
            {
                node.RemoveHighlights();

                bool hasInput = false;

                foreach (NodeSetting nodeSetting in node.NodeSettings)
                {
                    if (!nodeSetting.Input.Enabled || nodeSetting.Input.ConnectionPoint.ConnectionWire != null)
                    {
                        hasInput = true;

                        break;
                    }
                }

                if (!hasInput)
                {
                    node.HighlightError();

                    hasError = true;
                }
            }

            if (hasError)
            {
                return;
            }

            try
            {
                MainNode.Build();

                // sync _nodeStack to server and save

                Sandbox.Log.Error(System.Text.Json.JsonSerializer.Serialize(MainNode.GetJsonData()));

                Dictionary<string, object> jsonDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(System.Text.Json.JsonSerializer.Serialize(MainNode.GetJsonData()));

                Node.GetNodeFromJsonData<MainNode>(jsonDict);

                foreach (Node node in Nodes)
                {
                    node.Display();
                }
            }
            catch (Exception e)
            {
                Sandbox.Log.Error(e);
            }
        }
    }
}
