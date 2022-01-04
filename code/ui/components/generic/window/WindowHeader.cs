using System;

namespace TTTReborn.UI
{
    public partial class WindowHeader : Panel
    {
        public readonly WindowNavigationHeader NavigationHeader;

        public WindowDragHeader DragHeader;

        public WindowHeader(Window parent) : base(parent)
        {
            Parent = parent;

            StyleSheet.Load("/ui/components/generic/window/WindowHeader.scss");

            AddClass("windowheader");

            DragHeader = new WindowDragHeader(this, Parent as Window);
            NavigationHeader = new WindowNavigationHeader(DragHeader, Parent as Window);
        }
    }

    public partial class WindowNavigationHeader : PanelHeader
    {
        public Action<WindowNavigationHeader> OnCreateWindowHeader;

        public readonly Window Window;

        public WindowNavigationHeader(Sandbox.UI.Panel parent, Window window) : base(parent)
        {
            Parent = parent;
            Window = window;

            OnClose = (panelHeader) =>
            {
                Window.Enabled = false;
            };

            AddClass("windownavigationheader");
        }

        public override void OnCreateHeader()
        {
            OnCreateWindowHeader?.Invoke(this);

            base.OnCreateHeader();
        }
    }

    public partial class WindowDragHeader : Drag
    {
        public WindowDragHeader(Sandbox.UI.Panel parent, Window window) : base(parent)
        {
            Parent = parent;
            DragBasePanel = window;

            IsFreeDraggable = true;
        }
    }
}
