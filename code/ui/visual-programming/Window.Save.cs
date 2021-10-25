using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globals;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public static string VISUALPROGRAMMING_FILE_EXTENSION = ".vp.json";

        private FileSelection _currentFileSelection;

        public void Save()
        {
            _currentFileSelection?.Close();

            FileSelection fileSelection = FindRootPanel().Add.FileSelection();
            fileSelection.DefaultSelectionFileType = $"*{VISUALPROGRAMMING_FILE_EXTENSION}";
            fileSelection.OnAgree = () => OnAgreeSaveAs(fileSelection);
            fileSelection.DefaultSelectionPath = GetSettingsPathByData(Utils.Realm.Client);
            fileSelection.EnableFileNameEntry();
            fileSelection.Display();

            _currentFileSelection = fileSelection;
        }

        private string GetSettingsPathByData(Utils.Realm realm) => Utils.GetSettingsFolderPath(realm, null, "visualprogramming/");

        private void OnAgreeSaveAs(FileSelection fileSelection)
        {
            string fileName = fileSelection.FileNameEntry.Text;

            if (string.IsNullOrEmpty(fileName) || Window.Instance == null)
            {
                return;
            }

            fileSelection.Close();

            fileName = fileName.Split('/')[^1].Split('.')[0];

            if (!FileSystem.Data.FileExists(fileSelection.CurrentFolderPath + fileName + VISUALPROGRAMMING_FILE_EXTENSION))
            {
                SaveWorkspace(fileSelection.CurrentFolderPath, fileName);
            }
            else
            {
                AskOverwriteSelectedSettings(fileSelection.CurrentFolderPath, fileName, () => SaveWorkspace(fileSelection.CurrentFolderPath, fileName));
            }
        }

        internal static void AskOverwriteSelectedSettings(string folderPath, string fileName, Action onConfirm)
        {
            string fullFilePath = folderPath + fileName + VISUALPROGRAMMING_FILE_EXTENSION;

            DialogBox dialogBox = new DialogBox();
            dialogBox.SetTitle($"Overwrite '{fullFilePath}'");
            dialogBox.AddText($"Do you want to overwrite '{fullFilePath}' with the current settings? (If you agree, the settings defined in this file will be lost!)");
            dialogBox.OnAgree = () =>
            {
                onConfirm();

                dialogBox.Close();
            };
            dialogBox.OnDecline = () =>
            {
                dialogBox.Close();
            };

            Hud.Current.RootPanel.AddChild(dialogBox);

            dialogBox.Display();
        }

        private void SaveWorkspace(string path, string fileName)
        {
            Dictionary<string, object> jsonDict = new();

            // TODO add workspace settings to jsonDict as well

            List<Dictionary<string, object>> saveList = new();

            foreach (Node node in Nodes)
            {
                if (node.HasInput())
                {
                    continue;
                }

                saveList.Add(node.GetJsonData());
            }

            jsonDict.Add("Nodes", saveList);

            Player.TTTPlayer.SaveVisualProgramming(path, fileName, JsonSerializer.Serialize(jsonDict), Utils.Realm.Client);
        }
    }
}

namespace TTTReborn.Player
{
    using TTTReborn.UI.VisualProgramming;

    public partial class TTTPlayer
    {
        // TODO just save build data on server (valid data in a default place!)
        // [ServerCmd(Name = "ttt_visualprogramming_saveas_request")]
        // public static void RequestSaveVisualProgrammingAs(string filePath, string fileName, string jsonData, bool overwrite = false)
        // {
        //     if (!ConsoleSystem.Caller.HasPermission("visualprogramming"))
        //     {
        //         return;
        //     }

        //     if (overwrite || !FileSystem.Data.FileExists(filePath + fileName + VisualProgrammingWindow.VISUALPROGRAMMING_FILE_EXTENSION))
        //     {
        //         SaveVisualProgramming(filePath, fileName, jsonData);
        //     }
        //     else
        //     {
        //         ClientAskOverwriteSelectedVisualProgramming(To.Single(ConsoleSystem.Caller), filePath, fileName);
        //     }
        // }

        internal static void SaveVisualProgramming(string path, string fileName, string jsonData, Utils.Realm realm)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            path = Utils.GetSettingsFolderPath(realm, path);

            FileSystem.Data.WriteAllText(path + fileName + Window.VISUALPROGRAMMING_FILE_EXTENSION, jsonData);
        }
    }
}
