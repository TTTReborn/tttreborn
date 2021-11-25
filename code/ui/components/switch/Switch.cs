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

namespace TTTReborn.UI
{
    public partial class Switch : Sandbox.UI.Switch
    {
        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;

                SetClass("disable", _disabled);
            }
        }
        private bool _disabled = false;

        public Func<Sandbox.UI.MousePanelEvent, bool> OnCheck;

        public Switch(Sandbox.UI.Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/switch/Switch.scss");
        }

        protected override void OnClick(Sandbox.UI.MousePanelEvent e)
        {
            if (!Disabled && (OnCheck == null || OnCheck.Invoke(e)))
            {
                base.OnClick(e);
            }

            e.StopPropagation();
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class SwitchConstructor
    {
        public static Switch Switch(this PanelCreator self, string className = null, bool enabled = false)
        {
            Switch sw = new(self.panel);

            if (!string.IsNullOrEmpty(className))
            {
                sw.AddClass(className);
            }

            sw.Checked = enabled;

            return sw;
        }
    }
}
