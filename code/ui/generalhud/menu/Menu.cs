using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class Menu : RichPanel
    {
        public static Menu Instance;

        public readonly PanelContent MenuContent;

        private readonly MenuHeader _menuHeader;

        public Menu() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/menu/Menu.scss");

            _menuHeader = new MenuHeader(this);

            MenuContent = new PanelContent(this);
            MenuContent.OnPanelContentUpdated = (panelContentData) =>
            {
                _menuHeader.NavigationHeader.SetTitle(panelContentData.Title ?? "");
                _menuHeader.NavigationHeader.HomeButton.SetClass("disabled", panelContentData.ClassName == "home");
                _menuHeader.NavigationHeader.PreviousButton.SetClass("disabled", !MenuContent.CanPrevious);
                _menuHeader.NavigationHeader.NextButton.SetClass("disabled", !MenuContent.CanNext);
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
                Sandbox.UI.Label textLabel = panelContent.Add.Label("Loading...");

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
                panelContent.Add.Label("Switch:");

                panelContent.AddChild(new Switch());

                panelContent.Add.Label("DragDrop:");

                Sandbox.UI.Panel wrapperPanel = new(panelContent);

                Drop drop1 = new Drop(wrapperPanel);
                drop1.DragDropGroupName = "dnd";

                Drop drop2 = new Drop(wrapperPanel);
                drop2.DragDropGroupName = "dnd";

                new Drag(drop1).DragDropGroupName = "dnd";
                new Drag(drop1).DragDropGroupName = "dnd";
                new Drag(drop1).DragDropGroupName = "dnd";

                panelContent.Add.Label("Dropdown:");

                Dropdown dropdown = new Dropdown(panelContent);
                dropdown.TextLabel.Text = "Choose entry...";

                dropdown.AddOption("Test One", (option) =>
                {
                    Sandbox.Log.Error("Test One");
                });

                dropdown.AddOption("Test Two", (option) =>
                {
                    Sandbox.Log.Error("Test Two");
                });

                panelContent.AddChild(dropdown);
            }, "Testing", "testing");
        }
    }
}
