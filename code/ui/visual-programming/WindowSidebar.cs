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

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globals;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class WindowSidebar : Panel
    {
        public List<SidebarEntry> SidebarEntries = new();
        public bool IsFullOpened
        {
            get => _isFullOpened;
            private set
            {
                _isFullOpened = value;

                SetClass("open", _isFullOpened);
            }
        }
        private bool _isFullOpened = false;

        public WindowSidebar(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Dictionary<string, List<Type>> nodeTypeCategories = new();

            foreach (Type nodeType in Utils.GetTypesWithAttribute<Node, SpawnableAttribute>())
            {
                string category = Utils.GetAttribute<SpawnableAttribute>(nodeType).Categorie;

                if (!nodeTypeCategories.TryGetValue(category, out List<Type> nodeTypeList))
                {
                    nodeTypeList = new();

                    nodeTypeCategories.Add(category, nodeTypeList);
                }

                nodeTypeList.Add(nodeType);
            }

            foreach (KeyValuePair<string, List<Type>> keyValuePair in nodeTypeCategories)
            {
                Panel panel = new(this);
                panel.Add.Label(keyValuePair.Key, "category");

                foreach (Type nodeType in keyValuePair.Value)
                {
                    SidebarEntries.Add(new SidebarEntry(this, nodeType));
                }
            }
        }

        protected override void OnMouseOver(Sandbox.UI.MousePanelEvent e)
        {
            IsFullOpened = true;

            base.OnMouseOver(e);
        }

        protected override void OnMouseOut(Sandbox.UI.MousePanelEvent e)
        {
            IsFullOpened = false;

            base.OnMouseOut(e);
        }
    }

    public class SidebarEntry : Panel
    {
        public Sandbox.UI.Label TextLabel;

        public Type NodeType;

        private bool _mouseDown = false;

        private Node _currentNode;

        public SidebarEntry(WindowSidebar windowSidebar, Type nodeType) : base(windowSidebar)
        {
            NodeType = nodeType;

            TextLabel = Add.Label(Utils.GetLibraryName(nodeType), "entry");
        }

        protected override void OnMouseDown(Sandbox.UI.MousePanelEvent e)
        {
            if (!_mouseDown)
            {
                _currentNode = Utils.GetObjectByType<Node>(NodeType);
                Window.Instance.AddNode(_currentNode);

                _currentNode.Display();

                _mouseDown = true;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(Sandbox.UI.MousePanelEvent e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                _currentNode = null;
            }

            base.OnMouseUp(e);
        }

        public override void Tick()
        {
            if (_mouseDown)
            {
                _currentNode.Style.Left = Sandbox.UI.Length.Pixels(Mouse.Position.x);
                _currentNode.Style.Top = Sandbox.UI.Length.Pixels(Mouse.Position.y);
            }

            base.Tick();
        }
    }
}
