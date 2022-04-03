using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public struct TabMenuData
    {
        public Panel Panel { get; set; }
        public Action<Panel> OnLeave { get; set; }

        public TabMenuData(Panel panel, Action<Panel> onLeave)
        {
            Panel = panel;
            OnLeave = onLeave;
        }
    }

    [UseTemplate]
    public partial class TabMenu : Panel
    {
        public static TabMenu Instance { get; set; }

        public Panel MenuContainer { get; set; }
        public Panel SidebarPanel { get; set; }
        public Panel ContentPanel { get; set; }
        public string SelectedMenu { get; set; }
        public Button DefaultButton { get; set; }

        private readonly string _defaultPage = "scoreboard";
        private readonly string _defaultIcon = "score";

        private Dictionary<string, TabMenuData> TabMenuData { get; set; } = new();

        public TabMenu()
        {
            Instance = this;

            Init();
        }

        public void Init()
        {
            DefaultButton = AddMenu(_defaultPage, new Scoreboard(), _defaultIcon);

            AddMenu("results", new GameResultsMenu(), "bar_chart");

            TTTMenu tttmenu = new();

            AddMenu("menu", tttmenu, "settings", (pnl) =>
            {
                (pnl as TTTMenu).PopToHomePage();
            });

            SelectMenu(_defaultPage);
        }

        public Button AddMenu<T>(string name, T panel, string icon, Action<Panel> onLeave = null) where T : Panel
        {
            if (TabMenuData.ContainsKey(name) || panel == null)
            {
                return null;
            }

            TabMenuData.Add(name, new(panel, onLeave));
            ContentPanel.AddChild(panel);

            panel.Enabled(false);

            Button button = SidebarPanel.Add.ButtonWithIcon(null, icon, "icon", () =>
            {
                SelectMenu(name);
            });

            return button;
        }

        public void SelectMenu(string name)
        {
            if (SelectedMenu == name)
            {
                return;
            }

            foreach (TabMenuData data in TabMenuData.Values)
            {
                if (data.Panel != null && data.Panel.IsEnabled())
                {
                    data.OnLeave?.Invoke(data.Panel);
                    data.Panel.Enabled(false);
                }
            }

            SelectedMenu = name;

            if (!TabMenuData.TryGetValue(name, out TabMenuData tabMenuData) || tabMenuData.Panel == null)
            {
                return;
            }

            tabMenuData.Panel.Enabled(true);
        }

        public override void Tick()
        {
            base.Tick();

            if (!Input.Down(InputButton.Score))
            {
                if (SelectedMenu == _defaultPage)
                {
                    SetClass("fade-in", false);
                    DefaultButton.SetClass("close", false);
                    DefaultButton.Icon = _defaultIcon;
                }
                else
                {
                    DefaultButton.Icon = "close";
                    DefaultButton.SetClass("close", true);
                }

                return;
            }
            else
            {
                DefaultButton.SetClass("close", false);
                DefaultButton.Icon = _defaultIcon;
            }

            SetClass("fade-in", true);
            MenuContainer.SetClass("pop-in", true);
        }
    }
}
