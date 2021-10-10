using System;
namespace TTTReborn.UI.Menu
{
    using Sandbox.UI.Construct;

    public partial class Menu : Window
    {
        public static Menu Instance;

        public new bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;

                if (!IsEnabled)
                {
                    OpenHomepage();
                }
            }
        }

        public Menu() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/menu/Menu.scss");

            WindowHeader.NavigationHeader.OnCreateWindowHeader = (header) =>
            {
                Sandbox.UI.Button homeButton = new("home", "", () => OpenHomepage());
                homeButton.AddClass("home");

                header.AddChild(homeButton);

                Sandbox.UI.Button previousButton = header.Add.Button("<", "previous", () => WindowContent.Previous());
                Sandbox.UI.Button nextButton = header.Add.Button(">", "next", () => WindowContent.Next());

                homeButton.SetClass("disable", true);
                previousButton.SetClass("disable", true);
                nextButton.SetClass("disable", true);
            };

            WindowHeader.NavigationHeader.Reload();

            WindowContent.OnPanelContentUpdated = (panelContentData) =>
            {
                WindowHeader.NavigationHeader.SetTitle(panelContentData.Title ?? "");

                foreach (Sandbox.UI.Panel panel in WindowHeader.NavigationHeader.Children)
                {
                    if (panel is not Sandbox.UI.Button button)
                    {
                        continue;
                    }

                    if (button.HasClass("home"))
                    {
                        button.SetClass("disable", panelContentData.ClassName == "home");
                    }
                    else if (button.HasClass("previous"))
                    {
                        button.SetClass("disable", !WindowContent.CanPrevious);
                    }
                    else if (button.HasClass("next"))
                    {
                        button.SetClass("disable", !WindowContent.CanNext);
                    }
                }
            };

            OpenHomepage();

            IsDraggable = true;
            Enabled = false;
        }

        internal void OpenHomepage()
        {
            if (WindowContent.CurrentPanelContentData?.ClassName == "home")
            {
                return;
            }

            WindowContent.SetPanelContent((panelContent) =>
            {
                CreateMenuButton(panelContent, "settings", () => OpenSettings(panelContent));
                CreateMenuButton(panelContent, "keyboard", () => OpenKeybindings(panelContent));
                CreateMenuButton(panelContent, "science", () => OpenTesting(panelContent));
                CreateMenuButton(panelContent, "shopping_cart", () => OpenShopEditor(panelContent));
            }, "", "home");
        }

        private Sandbox.UI.Button CreateMenuButton(PanelContent panelContent, string iconName, Action onClick)
        {
            Sandbox.UI.Button button = panelContent.Add.ButtonWithIcon(iconName, "", "menuButton", onClick);
            button.AddClass("box-shadow");
            button.AddClass("background-color-secondary");
            button.AddClass("rounded");

            return button;
        }

        private void OpenKeybindings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("Bind TeamVoiceChat: '+ttt_teamvoicechat'");
                // panelContent.Add.Keybind("Press a key...").BoundCommand = "+ttt_teamvoicechat";

                panelContent.Add.Label("Bind Quickshop: '+ttt_quickshop'");
                // panelContent.Add.Keybind("Press a key...").BoundCommand = "+ttt_quickshop";

                panelContent.Add.Label("Bind Activate Role Button: '+ttt_activate_rb'");
                // panelContent.Add.Keybind("Press a key...").BoundCommand = "+ttt_activate_rb";
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
