using System.Collections.Generic;

namespace TTTReborn.UI.VisualProgramming
{
    public class VisualProgrammingWindow : Window
    {
        public static VisualProgrammingWindow Instance;

        public MainNode MainNode;
        public List<Node> Nodes = new();
        public NodeConnectionWire ActiveNodeConnectionWire;

        public VisualProgrammingWindow(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Instance = this;

            StyleSheet.Load("/ui/visual-programming/VisualProgrammingWindow.scss");

            AddClass("fullscreen");

            MainNode = AddNode<MainNode>();
            MainNode.Display();

            AddNode<ConditionalPlaceholderNode>().Display();
        }

        public T AddNode<T>() where T : Node, new()
        {
            T node = new T();

            Content.AddChild(node);
            Nodes.Add(node);

            return node;
        }
    }
}
