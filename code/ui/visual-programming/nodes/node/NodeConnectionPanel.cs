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

namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionPanel<T> : Panel where T : NodeConnectionPoint, new()
    {
        public Node Node
        {
            get => _node;
            set
            {
                _node = value;

                ConnectionPoint.Node = _node;
            }
        }
        private Node _node;

        public new bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                SetClass("disable", !_enabled);
            }
        }
        private bool _enabled = true;

        public T ConnectionPoint;

        public NodeConnectionPanel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            ConnectionPoint = new();

            AddChild(ConnectionPoint);

            AddClass("nodeconnectionpanel");
        }
    }
}
