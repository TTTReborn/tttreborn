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
using Sandbox.UI;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public partial class InfoFeed : Panel
    {
        public static InfoFeed Current;

        public InfoFeed() : base()
        {
            Current = this;

            StyleSheet.Load("/ui/generalhud/infofeed/InfoFeed.scss");
        }

        public virtual Sandbox.UI.Panel AddEntry(Client leftClient, string method)
        {
            InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

            bool isLeftLocal = leftClient == Local.Client;

            TTTPlayer leftPlayer = leftClient.Pawn as TTTPlayer;

            Label leftLabel = e.AddLabel(isLeftLocal ? "You" : leftClient.Name, "left");
            leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Color;

            e.AddLabel(method, "method");

            return e;
        }

        public virtual Panel AddEntry(string method, Color? color = null)
        {
            InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

            Label label = e.AddLabel(method, "method");
            label.Style.FontColor = color ?? Color.White;

            return e;
        }

        public virtual Sandbox.UI.Panel AddEntry(Client leftClient, Client rightClient, string method, string postfix = "")
        {
            InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

            bool isLeftLocal = leftClient == Local.Client;
            bool isRightLocal = rightClient == Local.Client;

            TTTPlayer leftPlayer = leftClient.Pawn as TTTPlayer;

            Label leftLabel = e.AddLabel(isLeftLocal ? "You" : leftClient.Name, "left");
            leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Color;

            e.AddLabel(method, "method");

            TTTPlayer rightPlayer = rightClient.Pawn as TTTPlayer;

            Label rightLabel = e.AddLabel(isRightLocal ? "You" : rightClient.Name, "right");
            rightLabel.Style.FontColor = rightPlayer.Role is NoneRole ? Color.White : rightPlayer.Role.Color;

            if (!string.IsNullOrEmpty(postfix))
            {
                e.AddLabel(postfix, "append");
            }

            return e;
        }
    }
}
