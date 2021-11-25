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

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class Tab : Panel
    {
        public readonly Label TitleLabel;

        public Action OnSelectTab { get; set; }
        public Action<PanelContent> CreateContent { get; set; }

        public object Value { get; set; }

        private readonly Tabs _parentTabs;

        public Tab(Sandbox.UI.Panel parent, Tabs parentTabs, string title, Action<PanelContent> createContent, object value = null, Action onSelectTab = null) : base(parent)
        {
            Parent = parent;
            _parentTabs = parentTabs;
            CreateContent = createContent;
            Value = value;
            OnSelectTab = onSelectTab;

            TitleLabel = Add.TryTranslationLabel(title, "title");
        }

        protected override void OnClick(MousePanelEvent e)
        {
            _parentTabs.OnSelectTab(this);
        }
    }
}
