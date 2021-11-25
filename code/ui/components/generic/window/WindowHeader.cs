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
