using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

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

        public void GoToShopEditor()
        {
            ShopEditorPage.ServerRequestShopEditorAccess();
        }

        public void GoToRoleSelectionEditor()
        {
            new VisualProgramming.Window(Hud.Current.RootPanel);
        }
    }
}
