using Sandbox.UI;
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
            IsShowing = false;
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
                panelContent.Add.ButtonWithIcon("keyboard", "", "menuButton", () => OpenKeybindings(panelContent));
                panelContent.Add.ButtonWithIcon("science", "", "menuButton", () => OpenTesting(panelContent));
            }, "", "home");
        }

        public void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                Panel buttonsWrapperPanel = panelContent.Add.Panel("wrapper");

                buttonsWrapperPanel.Add.Button("Save as", "fileselectionbutton", () =>
                {
                    FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                    fileSelection.DefaultSelectionPath = "/settings/";

                    fileSelection.OnAgree = () =>
                    {
                        string fileName = fileSelection.FileNameEntry.Text;

                        if (string.IsNullOrEmpty(fileName))
                        {
                            return;
                        }

                        Settings.SettingFunctions.SaveSettings(fileName.Split('/')[^1].Split('.')[0]);

                        fileSelection.Close();
                    };

                    fileSelection.EnableFileNameEntry();
                    fileSelection.Display();
                });

                buttonsWrapperPanel.Add.Button("Load from", "fileselectionbutton", () =>
                {
                    FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                    fileSelection.DefaultSelectionPath = "/settings/";

                    fileSelection.OnAgree = () =>
                    {
                        string fileName = fileSelection.SelectedEntry.FileNameLabel.Text;

                        if (string.IsNullOrEmpty(fileName))
                        {
                            return;
                        }

                        fileName = fileName.Split('/')[^1].Split('.')[0];

                        Settings.SettingFunctions.LoadSettings(fileName);

                        fileSelection.Close();

                        // Ask whether the player want to use the loaded settings as default ones
                        DialogBox dialogBox = new DialogBox();
                        dialogBox.TitleLabel.Text = "Default settings";
                        dialogBox.AddText($"Do you want to use '{fileName}.json' as the default settings? (If you agree, the current default settings will be overwritten!)");
                        dialogBox.OnAgree = () =>
                        {
                            Settings.SettingFunctions.SaveSettings();

                            dialogBox.Close();
                        };
                        dialogBox.OnDecline = () =>
                        {
                            dialogBox.Close();
                        };

                        FindRootPanel().AddChild(dialogBox);

                        dialogBox.Display();
                    };

                    fileSelection.Display();
                });
            }, "Settings", "settings");
        }

        public void OpenKeybindings(PanelContent menuContent)
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
                Menu.Instance.IsShowing = !Menu.Instance.IsShowing;
            }
        }
    }
}
