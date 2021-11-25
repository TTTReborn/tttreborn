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

using Sandbox;

using TTTReborn.Globals;

namespace TTTReborn.UI.VisualProgramming
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeSettingAttribute : LibraryAttribute
    {
        public NodeSettingAttribute(string name) : base("node_setting_" + name)
        {

        }
    }

    public abstract class NodeSetting : Panel
    {
        public string LibraryName { get; set; }

        public Node Node
        {
            get => _node;
            set
            {
                _node = value;

                if (Input != null)
                {
                    Input.Node = _node;
                }

                if (Output != null)
                {
                    Output.Node = _node;
                }
            }
        }
        private Node _node;

        public NodeConnectionPanel<NodeConnectionEndPoint> Input;
        public NodeConnectionPanel<NodeConnectionStartPoint> Output;
        public PanelContent Content;

        public NodeSetting() : base()
        {
            LibraryName = Utils.GetLibraryName(GetType());

            Input = new(this);
            Input.Node = Node;

            Content = new(this);

            Output = new(this);
            Output.Node = Node;

            AddClass("nodesetting");
        }

        public static NodeSettingAttribute GetAttribute<T>() where T : NodeSetting
        {
            return Library.GetAttribute(typeof(T)) as NodeSettingAttribute;
        }

        public NodeSettingAttribute GetAttribute()
        {
            return Library.GetAttribute(GetType()) as NodeSettingAttribute;
        }

        public void ToggleInput(bool toggle)
        {
            Input.Enabled = toggle;
        }

        public void ToggleOutput(bool toggle)
        {
            Output.Enabled = toggle;
        }
    }
}
