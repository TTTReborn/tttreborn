namespace TTTReborn.UI
{
    public partial class RichPanel : Panel
    {
        public RichPanel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/richpanel/RichPanel.scss");

            //AcceptsFocus = true;
        }
    }
}
