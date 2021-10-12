namespace TTTReborn.UI.VisualProgramming
{
    public class VisualProgrammingWindow : Window
    {
        public VisualProgrammingWindow(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/visual-programming/VisualProgrammingWindow.scss");

            AddClass("fullscreen");
        }
    }
}
