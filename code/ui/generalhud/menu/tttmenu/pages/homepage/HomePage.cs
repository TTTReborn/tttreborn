using Sandbox;
using Sandbox.UI;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class HomePage : Panel
    {
        private TranslationButton RoleSelectionEditorButton { get; set; }
        private TranslationButton ShopEditorButton { get; set; }

        public void GoToSettingsPage()
        {
            TTTMenu.Instance.AddPage(new SettingsPage());
        }

        public void GoToKeyBindingsPage()
        {
            TTTMenu.Instance.AddPage(new KeyBindingsPage());
        }


        public void GoToShopEditor()
        {
            // Call to server which sends down server data and then adds the ShopEditorPage.
            ShopEditorPage.ServerRequestShopEditorAccess();
        }

        public HomePage()
        {
            if (Local.Client.HasPermission("visualprogramming"))
            {
                RoleSelectionEditorButton.RemoveClass("inactive");
            }

            if (Local.Client.HasPermission("shopeditor"))
            {
                ShopEditorButton.RemoveClass("inactive");
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
