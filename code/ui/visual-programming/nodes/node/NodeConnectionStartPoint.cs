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

using Sandbox;

namespace TTTReborn.UI.VisualProgramming
{
    public class NodeConnectionStartPoint : NodeConnectionPoint
    {
        public bool IsDragging { get; internal set; } = false;

        public NodeConnectionStartPoint() : base()
        {

        }

        protected override void OnMouseDown(Sandbox.UI.MousePanelEvent e)
        {
            if (ConnectionWire != null)
            {
                return;
            }

            ConnectionWire = NodeConnectionWire.Create();
            ConnectionWire.StartPoint = this;
            Window.Instance.ActiveNodeConnectionWire = ConnectionWire;
            IsDragging = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(Sandbox.UI.MousePanelEvent e)
        {
            if (!IsDragging)
            {
                return;
            }

            if (ConnectionWire.EndPoint == null)
            {
                ConnectionWire.Delete(true);
            }

            IsDragging = false;
            Window.Instance.ActiveNodeConnectionWire = null;

            base.OnMouseUp(e);
        }

        public override void Tick()
        {
            if (!IsDragging)
            {
                return;
            }

            ConnectionWire.UpdateMousePosition(Mouse.Position);

            base.Tick();
        }
    }
}
