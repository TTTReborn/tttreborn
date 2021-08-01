using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class Menu : RichPanel
    {
        public static Menu Instance;

        private MenuHeader _menuHeader;

        public readonly PanelContent MenuContent;

        private readonly string _baseTitle = "Main Menu";

        public Menu() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/menu/Menu.scss");

            _menuHeader = new MenuHeader(this);
            _menuHeader.SetTitle(_baseTitle);

            MenuContent = new PanelContent(this);
            MenuContent.OnPanelContentUpdated = (panelContentData) =>
            {
                _menuHeader.PreviousButton.SetClass("disabled", !MenuContent.CanPrevious);
                _menuHeader.NextButton.SetClass("disabled", !MenuContent.CanNext);

                _menuHeader.SetTitle($"{_baseTitle} - {panelContentData.Title}");
            };

            MenuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
                panelContent.Add.Label("Test");
            }, "Test");

            MenuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("Test2");
                panelContent.Add.Label("Test2");
                panelContent.Add.Label("Test2");
                panelContent.Add.Label("Test2");
                panelContent.Add.Label("Test2");
                panelContent.Add.Label("Test2");
            }, "Test2");

            IsDraggable = true;
        }
    }
}
