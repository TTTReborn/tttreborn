namespace TTTReborn.UI.Menu
{
    using Sandbox;
    using Sandbox.UI.Construct;

    public partial class Menu : RichPanel
    {
        public static Menu Instance;

        public readonly PanelContent MenuContent;

        private readonly MenuHeader _menuHeader;

        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;

                if (!_isEnabled)
                {
                    OpenHomepage();
                }
            }
        }

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
            Enabled = false;
        }

        internal void OpenHomepage()
        {
            if (MenuContent.CurrentPanelContentData?.ClassName == "home")
            {
                return;
            }

            MenuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.ButtonWithIcon("settings", "", "menuButton", () => OpenSettings(panelContent));
                panelContent.Add.ButtonWithIcon("keyboard", "", "menuButton", () => OpenKeybindings(panelContent));
                panelContent.Add.ButtonWithIcon("science", "", "menuButton", () => OpenTesting(panelContent));
            }, "", "home");
        }

        private void OpenKeybindings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("Bind TeamVoiceChat:");
                panelContent.Add.Keybind("Press a key...").BoundCommand = "+ttt_teamvoicechat";

                panelContent.Add.Label("Bind Quickshop:");
                panelContent.Add.Keybind("Press a key...").BoundCommand = "+ttt_quickshop";

                panelContent.Add.Label("Bind Activate Role Button:");
                panelContent.Add.Keybind("Press a key...").BoundCommand = "+ttt_activate_rb";
            }, "Keybindings", "keybindings");
        }

        private void OpenTesting(PanelContent menuContent)
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

                Dropdown dropdown = panelContent.Add.Dropdown();
                dropdown.TextLabel.Text = "Choose entry...";

                dropdown.AddOption("Test One");
                dropdown.AddOption("Test Two");

                panelContent.AddChild(dropdown);

                panelContent.Add.Label("Keybind & DialogBox:");
                panelContent.Add.Keybind("Press a key...").BoundCommand = "+ttt_teamvoicechat";

                panelContent.Add.Label("FileSelection:");
                panelContent.Add.Button("Open FileSelection...", "fileselectionbutton", () => FindRootPanel().Add.FileSelection().Display());

                panelContent.Add.Label("Tabs:");

                Tabs tabs = panelContent.Add.Tabs();
                tabs.AddTab("Test1", (contentPanel) => contentPanel.Add.Label("Test1"));
                tabs.AddTab("Test2", (contentPanel) => contentPanel.Add.Label("Test2"));
            }, "Testing", "testing");
        }
    }
}

namespace TTTReborn.Player
{
    using Sandbox;

    using UI.Menu;

    public partial class TTTPlayer
    {
        private void TickMenu()
        {
            if (Input.Pressed(InputButton.Menu))
            {
                Menu.Instance.Enabled = !Menu.Instance.Enabled;
            }
        }
    }
}
