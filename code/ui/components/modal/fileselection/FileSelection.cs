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

        public PanelContent EntryPanelContent;
        public TextEntry FileNameEntry;

        public FileSelection(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Header.DragHeader.IsLocked = false;
            Header.DragHeader.IsFreeDraggable = true;

            StyleSheet.Load("/ui/components/modal/fileselection/FileSelection.scss");

            SetTitle(DefaultSelectionPath);

            OnDecline = () => Close();

            EntryPanelContent = new(Content);
            EntryPanelContent.AddClass("selection");

            FileNameEntry = Content.Add.TextEntry("");
            FileNameEntry.AddClass("filename");
            FileNameEntry.AddClass("hide");
            FileNameEntry.AddEventListener("onfocus", (panelEvent) =>
            {
                SelectedEntry?.SetClass("selected", false);
                SelectedEntry = null;
            });
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
            FileNameEntry.Text = "";

            CurrentFolderPath = path;
            SetTitle(path);
            SelectedEntry = null;

            EntryPanelContent.SetPanelContent((panelContent) =>
            {
                if (!path.Equals("/"))
                {
                    FileSelectionEntry fileSelectionEntry = panelContent.Add.FileSelectionEntry("../", "folder");
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
                    FileSelectionEntry fileSelectionEntry = panelContent.Add.FileSelectionEntry(Path.GetDirectoryName(folder + "/") + "/", "folder");
                    fileSelectionEntry.SetFileSelection(this);
                    fileSelectionEntry.IsFolder = true;
                }

                if (!FolderOnly)
                {
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
                        panelContent.Add.FileSelectionEntry(Path.GetFileName(file), GetIconByFileType(Path.GetExtension(file))).SetFileSelection(this);
                    }
                }
            });
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
            if (fileSelectionEntry == null || !fileSelectionEntry.IsFolder || FolderOnly)
            {
                OnSelectEntry?.Invoke(fileSelectionEntry);

                base.OnClickAgree();
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
            OnConfirm(SelectedEntry);
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
            FileSelection fileSelectionEntry = new(self.panel);
            fileSelectionEntry.DefaultSelectionPath = path;
            fileSelectionEntry.DefaultSelectionFileType = fileType;

            return fileSelectionEntry;
        }
    }
}
