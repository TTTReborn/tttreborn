namespace TTTReborn.UI.VisualProgramming
{
    public class VisualProgrammingWindow : Window
    {
        public VisualProgrammingWindow(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/visual-programming/VisualProgrammingWindow.scss");

            AddClass("fullscreen");

            Content.SetPanelContent((panelContent) =>
            {
                for (int i = 0; i < 10; i++)
                {
                    new Node(panelContent).Display();
                }
            });
        }
    }
}
