using Sandbox;
using Sandbox.UI;

using TTTReborn.Roles;

// TODO Rework

namespace TTTReborn.UI
{
    public partial class InfoFeed : Panel
    {
        public static InfoFeed Current { get; set; }

        public InfoFeed() : base()
        {
            Current = this;

            StyleSheet.Load("/ui/generalhud/infofeed/InfoFeed.scss");
        }

        public virtual Panel AddEntry(Client leftClient, string method)
        {
            InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

            bool isLeftLocal = leftClient == Local.Client;

            Player leftPlayer = leftClient.Pawn as Player;

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

        public virtual Panel AddEntry(Client leftClient, Client rightClient, string method, string postfix = "")
        {
            InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

            bool isLeftLocal = leftClient == Local.Client;
            bool isRightLocal = rightClient == Local.Client;

            Player leftPlayer = leftClient.Pawn as Player;

            Label leftLabel = e.AddLabel(isLeftLocal ? "You" : leftClient.Name, "left");
            leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Color;

            e.AddLabel(method, "method");

            Player rightPlayer = rightClient.Pawn as Player;

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
