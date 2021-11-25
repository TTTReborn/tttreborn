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

namespace TTTReborn.UI
{
    public partial class DropdownOptionHolder : Panel
    {
        public Dropdown RelatedPanel { get; private set; }

        public DropdownOptionHolder(Dropdown relatedPanel) : base()
        {
            RelatedPanel = relatedPanel;

            Hud.Current.RootPanel.AddChild(this);

            StyleSheet.Load("/ui/components/dropdown/DropdownOptionHolder.scss");
        }

        public override void Tick()
        {
            base.Tick();

            if (RelatedPanel.IsDeleted)
            {
                Delete(true);

                return;
            }

            if (!RelatedPanel.IsVisible)
            {
                Enabled = false;
            }

            if (!Enabled)
            {
                return;
            }

            Rect rect = RelatedPanel.Box.Rect;

            Style.Left = Length.Pixels(rect.left);
            Style.Top = Length.Pixels(rect.bottom);
            Style.Width = Length.Pixels(rect.width);
        }
    }
}
