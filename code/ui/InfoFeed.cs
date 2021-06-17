using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class InfoFeed : Panel
    {
        public static InfoFeed Current;

        public InfoFeed()
        {
            Current = this;

            StyleSheet.Load("/ui/InfoFeed.scss");
        }

        public virtual Panel AddEntry(Client leftClient, Client rightClient, string method, string postfix = "")
        {
            InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

            bool isLeftLocal = leftClient == Local.Client;
            bool isRightLocal = rightClient == Local.Client;

            Label leftLabel = e.AddLabel(isLeftLocal ? "You" : leftClient.Name, "left");
            leftLabel.Style.FontColor = (leftClient.Pawn as TTTPlayer).Role.Color;
            leftLabel.SetClass("me", isLeftLocal);

            e.AddLabel(method, "method");

            Label rightLabel = e.AddLabel(isRightLocal ? "You" : rightClient.Name, "right");
            rightLabel.Style.FontColor = (rightClient.Pawn as TTTPlayer).Role.Color;
            rightLabel.SetClass("me", isRightLocal);

            if (!string.IsNullOrEmpty(postfix))
            {
                e.AddLabel(postfix, "append");
            }

            return e;
        }
    }
}
