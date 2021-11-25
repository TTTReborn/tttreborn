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
    public partial class FileSelectionEntry : Panel
    {
        public readonly Label FileNameLabel;

        public readonly IconPanel FileTypeIcon;

        public bool IsFolder { get; set; } = false;

        private FileSelection _fileSelection;

        public FileSelectionEntry(Sandbox.UI.Panel parent) : base(parent)
        {
            FileTypeIcon = Add.Icon(null, "filetype");
            FileNameLabel = Add.Label("", "filename");
        }

        public void SetFileSelection(FileSelection fileSelection)
        {
            _fileSelection = fileSelection;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            _fileSelection?.OnSelect(this);
        }

        protected override void OnDoubleClick(MousePanelEvent e)
        {
            _fileSelection?.OnConfirm(this);
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class FileSelectionEntryConstructor
    {
        public static FileSelectionEntry FileSelectionEntry(this PanelCreator self, string text = "", string icon = null)
        {
            FileSelectionEntry fileSelectionEntry = new(self.panel);
            fileSelectionEntry.FileNameLabel.Text = text;
            fileSelectionEntry.FileTypeIcon.Text = icon;

            return fileSelectionEntry;
        }
    }
}
