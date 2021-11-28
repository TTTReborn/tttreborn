using System.Collections.Generic;

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

        public Window(Sandbox.UI.Panel parent = null) : base(parent)
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

                // TODO build from server's Stack
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
    }
}
