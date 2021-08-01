using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class Menu : RichPanel
    {
        public static Menu Instance;

        private MenuHeader _menuHeader;

        public readonly PanelContent MenuContent;

        public Menu() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/menu/Menu.scss");

            _menuHeader = new MenuHeader(this);

            MenuContent = new PanelContent(this);
            MenuContent.OnPanelContentUpdated = (panelContentData) =>
            {
                _menuHeader.SetTitle(panelContentData.Title ?? "");
                _menuHeader.PreviousButton.SetClass("disabled", !MenuContent.CanPrevious);
                _menuHeader.NextButton.SetClass("disabled", !MenuContent.CanNext);
            };

            MenuContent.SetPanelContent((panelContent) =>
            {
                for (int i = 0; i < 12; i++)
                {
                    Button button = new Button("settings", "", () => OpenSettings(panelContent));
                    button.AddClass("menuButton");

                    panelContent.AddChild(button);
                }
            }, "", "mainmenu");

            IsDraggable = true;
        }

        private void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
            }, "Settings", "settings");
        }
    }
}
