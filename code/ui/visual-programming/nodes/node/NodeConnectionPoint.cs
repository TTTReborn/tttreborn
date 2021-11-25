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
    public class NodeConnectionPoint : Panel
    {
        public Node Node { get; set; }

        public NodeConnectionWire ConnectionWire
        {
            get => _connectionWire;
            internal set
            {
                _connectionWire = value;

                SetClass("connected", _connectionWire != null);
            }
        }
        private NodeConnectionWire _connectionWire;

        public Vector2 Position
        {
            get
            {
                Rect rect = Box.Rect;

                return new Vector2(rect.left + rect.width * 0.5f, rect.top + rect.height * 0.5f);
            }
        }

        public NodeConnectionPoint() : base()
        {
            AddClass("nodeconnectionpoint");
        }

        public int GetSettingsIndex()
        {
            int index = 0;

            for (int i = 0; i < Node.NodeSettings.Count; i++)
            {
                if (Node.NodeSettings[i].Input.ConnectionPoint == this)
                {
                    return i;
                }
            }

            return index;
        }
    }
}
