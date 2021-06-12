namespace Sandbox.UI
{
    public partial class InfoFeed : Panel
    {
        public static InfoFeed Current;

        public InfoFeed()
        {
            Current = this;

            StyleSheet.Load( "/ui/InfoFeed.scss" );
        }

        public virtual Panel AddEntry(ulong leftSteamId, string leftName, ulong rightSteamId, string rightName, string method)
        {
            InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

            e.Left.Text = leftName;
            e.Left.SetClass("me", leftSteamId == (Local.Client?.SteamId));

            e.Method.Text = method;

            e.Right.Text = rightName;
            e.Right.SetClass("me", rightSteamId == (Local.Client?.SteamId));

            return e;
        }
    }
}
