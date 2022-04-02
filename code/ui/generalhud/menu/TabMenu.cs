using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
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

        private Dictionary<string, Panel> Panels { get; set; } = new();

        public TabMenu()
        {
            Instance = this;

            Init();
        }

        public void Init()
        {
            DefaultButton = AddMenu(_defaultPage, new Scoreboard(), _defaultIcon);

            AddMenu("results", new GameResultsMenu(), "bar_chart");
            AddMenu("menu", new TTTMenu(), "settings");

            SelectMenu(_defaultPage);
        }

        public Button AddMenu<T>(string name, T panel, string icon) where T : Panel
        {
            if (Panels.ContainsKey(name) || panel == null)
            {
                return null;
            }

            Panels.Add(name, panel);
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

            foreach (Panel pnl in ContentPanel.Children)
            {
                pnl.Enabled(false);
            }

            SelectedMenu = name;

            if (!Panels.TryGetValue(name, out Panel panel) || panel == null)
            {
                return;
            }

            panel.Enabled(true);
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
