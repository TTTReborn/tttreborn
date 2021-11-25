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

using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class InfoFeedEntry : Panel
    {
        private readonly List<Label> _labels = new();

        private readonly RealTimeSince _timeSinceBorn = 0;

        public InfoFeedEntry()
        {
            AddClass("background-color-primary");
            AddClass("text-shadow");
            AddClass("opacity-heavy");
            AddClass("rounded");
        }

        public Label AddLabel(string text, string classname)
        {
            Label label = Add.Label(text, classname);

            _labels.Add(label);

            return label;
        }

        public override void Tick()
        {
            base.Tick();

            if (_timeSinceBorn > 6)
            {
                Delete();
            }
        }
    }
}
