using System;
using System.IO;

using Sandbox;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class FileSelection : DialogBox
    {
        public string SelectedFilePath { get; private set; }

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

        private string _currentFolderPath = DEFAULT_SELECTION_PATH;

        public FileSelection() : base()
        {
            HeaderPanel.IsFreeDraggable = true;

            StyleSheet.Load("/ui/components/modal/fileselection/FileSelection.scss");

            TitleLabel.Text = DefaultSelectionPath;

            OnDecline = (panel) => panel.Close();
        }

        public override void Display()
        {
            base.Display();

            CreateTreeView(DefaultSelectionPath);
        }

        public void CreateTreeView(string path)
        {
            _currentFolderPath = path;

            TitleLabel.Text = path;

            ContentPanel.DeleteChildren(true);

            if (!path.Equals("/"))
            {
                FileSelectionEntry fileSelectionEntry = ContentPanel.Add.FileSelectionEntry("../", "folder");
                fileSelectionEntry.SetFileSelection(this);
                fileSelectionEntry.IsFolder = true;
            }

            foreach (string folder in FileSystem.Mounted.FindDirectory(path))
            {
                FileSelectionEntry fileSelectionEntry = ContentPanel.Add.FileSelectionEntry(Path.GetDirectoryName(folder + "/") + "/", "folder");
                fileSelectionEntry.SetFileSelection(this);
                fileSelectionEntry.IsFolder = true;
            }

            if (FolderOnly)
            {
                return;
            }

            foreach (string file in FileSystem.Mounted.FindFile(path, DefaultSelectionFileType))
            {
                ContentPanel.Add.FileSelectionEntry(Path.GetFileName(file), GetIconByFileType(Path.GetExtension(file))).SetFileSelection(this);
            }
        }

        public void OnSelect(FileSelectionEntry fileSelectionEntry)
        {
            SelectedEntry?.SetClass("selected", false);

            SelectedEntry = fileSelectionEntry;

            SelectedEntry.SetClass("selected", true);
        }

        public void OnConfirm(FileSelectionEntry fileSelectionEntry)
        {
            if (!fileSelectionEntry.IsFolder || FolderOnly)
            {
                OnSelectEntry?.Invoke(fileSelectionEntry);
            }
            else // go deeper
            {
                if (fileSelectionEntry.FileNameLabel.Text.Equals("../"))
                {
                    CreateTreeView(Path.GetDirectoryName(_currentFolderPath.TrimEnd('/')).Replace('\\', '/'));
                }
                else
                {
                    CreateTreeView(_currentFolderPath + fileSelectionEntry.FileNameLabel.Text);
                }
            }
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
