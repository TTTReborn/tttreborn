namespace TTTReborn.UI.VisualProgramming
{
    public class VisualProgrammingWindow : Window
    {
        public MainNode MainNode;

        public VisualProgrammingWindow(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/visual-programming/VisualProgrammingWindow.scss");

            AddClass("fullscreen");

            Content.SetPanelContent((panelContent) =>
            {
                new MainNode(panelContent).Display();
            });
        }
    }
}
