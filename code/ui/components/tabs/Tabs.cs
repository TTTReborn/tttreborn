using System;
using System.Collections.Generic;

using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Tabs : TTTPanel
    {
        public readonly List<Tab> TabList = new();

        public readonly Panel Header;
        public readonly PanelContent PanelContent;

        public Action<Tab> OnTabSelected { get; set; }

        public Tab SelectedTab { get; private set; }

        public Tabs() : base()
        {
            StyleSheet.Load("/ui/components/tabs/Tabs.scss");

            Header = Add.Panel("header");

            PanelContent = new PanelContent(this);
            PanelContent.AddClass("content");
        }

        public Tab AddTab(string title, Action<PanelContent> createContent, object value = null, Action onSelectTab = null)
        {
            Tab tab = new Tab(Header, this, title, createContent, value, onSelectTab);

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

            SelectedTab?.SetClass("selected", false);

            SelectedTab = tab;

            SelectedTab.SetClass("selected", true);
            PanelContent.SetPanelContent(SelectedTab.CreateContent, SelectedTab.TitleLabel.Text);
            SelectedTab.OnSelectTab?.Invoke();

            OnTabSelected?.Invoke(SelectedTab);
        }

        public void SelectByValue(object data)
        {
            foreach (Tab tab in TabList)
            {
                if (tab.Value.Equals(data))
                {
                    OnSelectTab(tab);

                    return;
                }
            }
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
