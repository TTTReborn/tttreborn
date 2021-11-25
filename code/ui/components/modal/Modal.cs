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
    public partial class Modal : Window
    {
        public Action<Modal> OnDisplay;
        public bool IsDeletedOnClose;

        public Modal(Sandbox.UI.Panel parent = null, bool isDeletedOnClose = true) : base(parent)
        {
            IsDeletedOnClose = isDeletedOnClose;

            Action<PanelHeader> action = Header.NavigationHeader.OnClose;

            Header.NavigationHeader.OnClose = (panelHeader) =>
            {
                action?.Invoke(panelHeader);

                if (IsDeletedOnClose)
                {
                    Delete(true);
                }
            };

            Enabled = false;
        }

        public virtual void Display()
        {
            OnDisplay?.Invoke(this);

            Enabled = true;
        }

        public virtual void Close()
        {
            Enabled = false;

            Header.NavigationHeader.OnClose?.Invoke(Header.NavigationHeader);
        }
    }
}
