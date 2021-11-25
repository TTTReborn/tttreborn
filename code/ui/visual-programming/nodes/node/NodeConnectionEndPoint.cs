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
    public class NodeConnectionEndPoint : NodeConnectionPoint
    {
        public NodeConnectionEndPoint() : base()
        {

        }

        protected override void OnMouseOver(Sandbox.UI.MousePanelEvent e)
        {
            NodeConnectionWire currentConnectionWire = Window.Instance.ActiveNodeConnectionWire;

            if (ConnectionWire != null || currentConnectionWire == null || currentConnectionWire.StartPoint.Node == Node)
            {
                return;
            }

            ConnectionWire = currentConnectionWire;
            ConnectionWire.EndPoint = this;

            Window.Instance.ActiveNodeConnectionWire = null;
            ConnectionWire.StartPoint.IsDragging = false;

            base.OnMouseOver(e);
        }
    }
}
