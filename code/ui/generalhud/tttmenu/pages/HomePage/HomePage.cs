using Sandbox.UI;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class HomePage : Panel
    {
        public void GoToComponentTesting()
        {
            TTTMenu.Instance.AddPage(new ComponentTestingPage());
        }

        public void GoToKeyBindings()
        {
            TTTMenu.Instance.AddPage(new KeyBindingsPage());
        }

        public void GoToRoleSelectionEditor()
        {
            // Launches a full screen panel.
            new VisualProgramming.Window(Hud.Current.RootPanel);
        }
    }
}
