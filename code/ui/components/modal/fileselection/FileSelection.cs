using System;
using System.Collections.Generic;
using System.IO;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class FileSelection : DialogBox
    {
        public bool IsDataFolder { get; set; } = true;

        public string DefaultSelectionPath
        {
            get => _defaultSelectionPath ?? DEFAULT_SELECTION_PATH;
            set
            {
                _defaultSelectionPath = value;
            }
        }
        private string _defaultSelectionPath;

        public const string DEFAULT_SELECTION_PATH = "/";

        public string DefaultSelectionFileType
        {
            get => _defaultSelectionFileType ?? DEFAULT_SELECTION_FILETYPE;
            set
            {
                _defaultSelectionFileType = value;
            }
        }
        private string _defaultSelectionFileType;

        public const string DEFAULT_SELECTION_FILETYPE = "*";

        public bool FolderOnly { get; set; } = false;

        public FileSelectionEntry SelectedEntry { get; private set; }

        public Action<FileSelectionEntry> OnSelectEntry { get; set; }

        public string CurrentFolderPath = DEFAULT_SELECTION_PATH;

        private readonly Sandbox.UI.Panel _selectionPanel;
        public readonly TextEntry FileNameEntry;

        public FileSelection() : base()
        {
            HeaderPanel.IsLocked = false;
            HeaderPanel.IsFreeDraggable = true;

            StyleSheet.Load("/ui/components/modal/fileselection/FileSelection.scss");

            TitleLabel.Text = DefaultSelectionPath;

            OnDecline = () => Close();

            _selectionPanel = ContentPanel.Add.Panel("selection");

            FileNameEntry = ContentPanel.Add.TextEntry("");
            FileNameEntry.AddClass("filename");
            FileNameEntry.AddClass("hide");
        }

        public void EnableFileNameEntry(bool enable = true)
        {
            FileNameEntry.SetClass("hide", !enable);
        }

        public override void Display()
        {
            base.Display();

            CreateTreeView(DefaultSelectionPath);
        }

        public void CreateTreeView(string path)
        {
            CurrentFolderPath = path;
            TitleLabel.Text = path;
            SelectedEntry = null;

            _selectionPanel.DeleteChildren(true);

            if (!path.Equals("/"))
            {
                FileSelectionEntry fileSelectionEntry = _selectionPanel.Add.FileSelectionEntry("../", "folder");
                fileSelectionEntry.SetFileSelection(this);
                fileSelectionEntry.IsFolder = true;
            }

            IEnumerable<string> folders;

            if (IsDataFolder)
            {
                folders = FileSystem.Data.FindDirectory(path);
            }
            else
            {
                folders = FileSystem.Mounted.FindDirectory(path);
            }

            foreach (string folder in folders)
            {
                FileSelectionEntry fileSelectionEntry = _selectionPanel.Add.FileSelectionEntry(Path.GetDirectoryName(folder + "/") + "/", "folder");
                fileSelectionEntry.SetFileSelection(this);
                fileSelectionEntry.IsFolder = true;
            }

            if (FolderOnly)
            {
                return;
            }

            IEnumerable<string> files;

            if (IsDataFolder)
            {
                files = FileSystem.Data.FindFile(path, DefaultSelectionFileType);
            }
            else
            {
                files = FileSystem.Mounted.FindFile(path, DefaultSelectionFileType);
            }

            foreach (string file in files)
            {
                _selectionPanel.Add.FileSelectionEntry(Path.GetFileName(file), GetIconByFileType(Path.GetExtension(file))).SetFileSelection(this);
            }
        }

        public void OnSelect(FileSelectionEntry fileSelectionEntry)
        {
            SelectedEntry?.SetClass("selected", false);

            SelectedEntry = fileSelectionEntry;
            FileNameEntry.Text = SelectedEntry.FileNameLabel.Text;

            if (FolderOnly || !SelectedEntry.IsFolder)
            {
                FileNameEntry.Text = SelectedEntry.FileNameLabel.Text;
            }

            SelectedEntry.SetClass("selected", true);
        }

        public void OnConfirm(FileSelectionEntry fileSelectionEntry)
        {
            if (!fileSelectionEntry.IsFolder || FolderOnly)
            {
                OnSelectEntry?.Invoke(fileSelectionEntry);
                OnAgree?.Invoke();
            }
            else // go deeper
            {
                if (fileSelectionEntry.FileNameLabel.Text.Equals("../"))
                {
                    string path = Path.GetDirectoryName(CurrentFolderPath.TrimEnd('/')).Replace('\\', '/');

                    if (!path.Equals("/"))
                    {
                        path += "/";
                    }

                    CreateTreeView(path);
                }
                else
                {
                    CreateTreeView(CurrentFolderPath + fileSelectionEntry.FileNameLabel.Text);
                }
            }
        }

        public override void OnClickAgree()
        {
            if (SelectedEntry != null)
            {
                if (!FolderOnly && SelectedEntry.IsFolder)
                {
                    OnConfirm(SelectedEntry);

                    return;
                }

                OnSelectEntry?.Invoke(SelectedEntry);
            }

            base.OnClickAgree();
        }

        public static string GetIconByFileType(string fileType)
        {
            switch (fileType)
            {
                case "txt":
                    return "text_snippet";
                case "json":
                    return "settings_applications";
                default:
                    return "description";
            }
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class FileSelectionConstructor
    {
        public static FileSelection FileSelection(this PanelCreator self, string path = null, string fileType = null)
        {
            FileSelection fileSelectionEntry = self.panel.AddChild<FileSelection>();
            fileSelectionEntry.DefaultSelectionPath = path;
            fileSelectionEntry.DefaultSelectionFileType = fileType;

            return fileSelectionEntry;
        }
    }
}
