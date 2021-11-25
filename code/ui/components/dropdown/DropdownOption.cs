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

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class DropdownOption : Panel
    {
        public readonly Dropdown Dropdown;

        public readonly TranslationLabel TextLabel;

        public object Data { get; set; }

        public Action<Panel> OnSelect { get; set; }

        public DropdownOption(Dropdown dropdown, Sandbox.UI.Panel parent = null, string text = "", object data = null, params object[] translationData) : base(parent)
        {
            Dropdown = dropdown;
            TextLabel = Add.TryTranslationLabel(text, "optiontext", translationData);
            Data = data;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            OnSelect?.Invoke(this);
            Dropdown.OnSelectDropdownOption(this);
        }
    }
}
