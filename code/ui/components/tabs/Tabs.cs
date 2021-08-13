using System;
using System.Collections.Generic;

using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Tabs : TTTPanel
    {
        public readonly List<Tab> TabList = new();

        public readonly Panel Header;
        public readonly Panel Content;

        public Tab SelectedTab { get; private set; }

        public Tabs() : base()
        {
            StyleSheet.Load("/ui/components/tabs/Tabs.scss");

            Header = Add.Panel("header");
            Content = Add.Panel("content");
        }

        public Tab AddTab(string title, Action<Panel> content, Action onSelectTab = null)
        {
            Tab tab = new Tab(Header, this);
            tab.Content = content;
            tab.OnSelectTab = onSelectTab;
            tab.SetTitle(title);

            TabList.Add(tab);

            if (TabList.Count == 1)
            {
                OnSelectTab(tab);
            }

            return tab;
        }

        public virtual void OnSelectTab(Tab tab)
        {
            if (tab == SelectedTab)
            {
                return;
            }

            Content.DeleteChildren(true);

            SelectedTab?.SetClass("selected", false);

            SelectedTab = tab;

            SelectedTab.SetClass("selected", true);
            SelectedTab.Content?.Invoke(Content);
            SelectedTab.OnSelectTab?.Invoke();
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TabsConstructor
    {
        public static Tabs Tabs(this PanelCreator self)
        {
            return self.panel.AddChild<Tabs>();
        }
    }
}
