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

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class WIPDisclaimer : Panel
    {
        public static WIPDisclaimer Instance { get; set; }

        public WIPDisclaimer() : base()
        {
            Instance = this;

            Panel wrapper = new(this);
            wrapper.Style.FlexDirection = FlexDirection.Column;
            wrapper.Style.TextAlign = TextAlign.Center;
            wrapper.AddClass("centered-vertical-10");
            wrapper.AddClass("opacity-medium");
            wrapper.AddClass("text-color-info");
            wrapper.AddClass("text-shadow");

            wrapper.Add.Label("TTT Reborn is work-in-progress!");
            wrapper.Add.Label("Everything you see is subject to change or is actively being worked on!");
            wrapper.Add.Label("Our project is open source, consider contributing at github.com/TTTReborn");

            AddClass("fullscreen");
        }
    }
}
