// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;
using System.Collections.Generic;

namespace TTTReborn.UI
{
    public partial class Tabs : Panel
    {
        public readonly List<Tab> TabList = new();

        public readonly Sandbox.UI.Panel Header;
        public readonly PanelContent PanelContent;

        public Action<Tab> OnTabSelected { get; set; }

        public Tab SelectedTab { get; private set; }

        public Tabs(Sandbox.UI.Panel parent = null) : base(parent)
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
            return new(self.panel);
        }
    }
}
