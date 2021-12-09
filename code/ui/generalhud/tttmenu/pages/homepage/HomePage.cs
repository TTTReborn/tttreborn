using Sandbox;
using Sandbox.UI;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class HomePage : Panel
    {
        private TranslationButton RoleSelectionEditorButton { get; set; }

        public void GoToSettingsPage()
        {
            TTTMenu.Instance.AddPage(new SettingsPage());
        }

        public void GoToKeyBindingsPage()
        {
            TTTMenu.Instance.AddPage(new KeyBindingsPage());
        }

        public HomePage()
        {
            if (Local.Client.HasPermission("visualprogramming"))
            {
                RoleSelectionEditorButton.RemoveClass("inactive");
            }
        }

        public void GoToComponentTesting()
        {
            TTTMenu.Instance.AddPage(new ComponentTestingPage());
        }

        public void GoToRoleSelectionEditor()
        {
            // Ends up launching a new panel on the GeneralHud layer.
            NodeStack.ServerRequestStack();
        }
    }
}