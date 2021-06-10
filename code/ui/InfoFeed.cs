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

        public virtual Panel AddEntry(ulong lsteamid, string left, ulong rsteamid, string right, string method)
        {
            var e = Current.AddChild<InfoFeedEntry>();

            e.Left.Text = left;
            e.Left.SetClass("me", lsteamid == (Local.Client?.SteamId));

            e.Method.Text = method;

            e.Right.Text = right;
            e.Right.SetClass("me", rsteamid == (Local.Client?.SteamId));

            return e;
        }
    }
}
