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

            _nodeStack = new NodeStack(); // TODO move to server later
        }

        public T AddNode<T>() where T : Node, new()
        {
            T node = new T();

            Content.AddChild(node);
            Nodes.Add(node);

            return node;
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
                    if (!nodeSetting.Input.Enabled)
                    {
                        hasInput = true;

                        break;
                    }

                    foreach (NodeConnectionPoint nodeConnectionPoint in nodeSetting.Input.ConnectionPoints)
                    {
                        if (nodeConnectionPoint.ConnectionWire != null)
                        {
                            hasInput = true;

                            break;
                        }
                    }

                    if (hasInput)
                    {
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
            }
            catch (Exception) { }
        }
    }
}
