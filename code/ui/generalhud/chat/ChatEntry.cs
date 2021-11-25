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

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class ChatEntry : Panel
    {
        public readonly Sandbox.UI.Panel Message;
        public readonly Image Avatar;
        public readonly Label Header;
        public readonly Label Content;
        public ChatBox.Channel Channel;

        public ChatEntry() : base()
        {
            Avatar = Add.Image();
            Avatar.AddClass("avatar");
            Avatar.AddClass("circular");

            Message = Add.Panel("message");

            Header = Message.Add.Label();
            Header.AddClass("header");
            Header.AddClass("text-shadow");

            Content = Message.Add.Label();
            Content.AddClass("content");
            Content.AddClass("text-shadow");
        }
    }
}
