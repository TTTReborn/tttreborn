using System.Collections.Generic;

namespace TTTReborn.UI.VisualProgramming
{
    public class VisualProgrammingWindow : Window
    {
        public static VisualProgrammingWindow Instance;

        public MainNode MainNode;
        public List<Node> Nodes = new();
        public NodeConnectionWire ActiveNodeConnectionWire;
        public bool IsTesting { get; private set; } = false;

        public VisualProgrammingWindow(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Instance = this;

            StyleSheet.Load("/ui/visual-programming/VisualProgrammingWindow.scss");

            AddClass("fullscreen");

            Header.NavigationHeader.OnCreateWindowHeader = (header) =>
            {
                Sandbox.UI.Button playButton = new("play_arrow", "", () => Test());
                playButton.AddClass("play");

                header.AddChild(playButton);
            };

            Header.NavigationHeader.Reload();

            MainNode = AddNode<MainNode>();
            MainNode.Display();

            AddNode<RoleSelectionNode>().Display();
        }

        public T AddNode<T>() where T : Node, new()
        {
            T node = new T();

            Content.AddChild(node);
            Nodes.Add(node);

            return node;
        }

        public void Evaluate()
        {
            MainNode.Evaluate();
        }

        public void Test()
        {
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
                }
            }

            IsTesting = true;

            Evaluate();

            IsTesting = false;
        }
    }
}
