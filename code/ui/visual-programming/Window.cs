using System.Collections.Generic;

using TTTReborn.VisualProgramming;

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

        private NodeStack _nodeStack;

        public Window(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Instance = this;

            StyleSheet.Load("/ui/visual-programming/Window.scss");

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

            Content.SetPanelContent((panelContent) =>
            {
                Workspace = new(panelContent);
                Sidebar = new(panelContent);

                MainNode = AddNode<MainNode>();
                MainNode.Display();

                AddNode<RoleSelectionNode>().Display();
                AddNode<RoleSelectionNode>().Display();
                AddNode<PercentageSelectionNode>().Display();
            });

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
            Workspace.AddChild(node);
            Nodes.Add(node);
        }
    }
}
