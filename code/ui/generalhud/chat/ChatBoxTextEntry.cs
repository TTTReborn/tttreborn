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

using Sandbox;

namespace TTTReborn.UI
{
    public partial class ChatBoxTextEntry : Sandbox.UI.TextEntry
    {
        public ChatBoxTextEntry(Sandbox.UI.Panel parent = null) : base()
        {
            Parent = parent ?? Parent;
        }

        public override void OnButtonTyped(string button, KeyModifiers km)
        {
            if (button.Equals("tab"))
            {
                ChatBox.Instance.OnTab();

                return;
            }

            base.OnButtonTyped(button, km);
        }
    }
}
