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
                _menuHeader.HomeButton.SetClass("disabled", panelContentData.ClassName == "home");
                _menuHeader.PreviousButton.SetClass("disabled", !MenuContent.CanPrevious);
                _menuHeader.NextButton.SetClass("disabled", !MenuContent.CanNext);
            };

            OpenHomepage();

            IsDraggable = true;
        }

        public void OpenHomepage()
        {
            if (MenuContent.CurrentPanelContentData?.ClassName == "home")
            {
                return;
            }

            MenuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.ButtonWithIcon("settings", "", "menuButton", () => OpenSettings(panelContent));
                panelContent.Add.ButtonWithIcon("change_circle", "", "menuButton", () => OpenChanges(panelContent));
                panelContent.Add.ButtonWithIcon("science", "", "menuButton", () => OpenTesting(panelContent));
            }, "", "home");
        }

        public void OpenSettings(PanelContent menuContent)
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

        public void OpenChanges(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                Label textLabel = panelContent.Add.Label("Loading...");

                Sandbox.Internal.Http http = new Sandbox.Internal.Http(new System.Uri("https://commits.facepunch.com/r/sbox"));
                http.GetStringAsync().ContinueWith(result =>
                {
                    textLabel.Text = result.Result;
                });
            }, "Http", "http");
        }

        public void OpenTesting(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                panelContent.AddChild(new Switch());
            }, "Testing", "testing");
        }
    }
}
