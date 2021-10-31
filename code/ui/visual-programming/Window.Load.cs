using System.Collections.Generic;
using System.Text.Json;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globals;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public void Load()
        {
            _currentFileSelection?.Close();

            FileSelection fileSelection = FindRootPanel().Add.FileSelection();
            fileSelection.DefaultSelectionFileType = $"*{VISUALPROGRAMMING_FILE_EXTENSION}";
            fileSelection.OnAgree = () => OnAgreeLoadVisualProgrammingFrom(fileSelection);
            fileSelection.DefaultSelectionPath = GetSettingsPathByData(Utils.Realm.Client);
            fileSelection.Display();

            _currentFileSelection = fileSelection;
        }

        private void OnAgreeLoadVisualProgrammingFrom(FileSelection fileSelection)
        {
            if (fileSelection.SelectedEntry == null)
            {
                return;
            }

            string fileName = fileSelection.SelectedEntry.FileNameLabel.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            fileName = fileName.Split('/')[^1].Split('.')[0];

            if (Window.Instance == null)
            {
                fileSelection.Close();

                return;
            }

            Dictionary<string, object> jsonData = Player.TTTPlayer.LoadVisualProgramming(fileSelection.CurrentFolderPath, fileName, Utils.Realm.Client);

            if (jsonData == null)
            {
                Log.Error($"VisualProgramming file '{fileSelection.CurrentFolderPath}{fileName}{VISUALPROGRAMMING_FILE_EXTENSION}' can't be loaded.");

                return;
            }

            LoadWorkspace(jsonData);

            fileSelection.Close();
        }

        private void LoadWorkspace(Dictionary<string, object> jsonData)
        {
            // TODO load workspace settings from jsonData as well

            jsonData.TryGetValue("Nodes", out object saveListJson);

            if (saveListJson == null)
            {
                return;
            }

            List<Dictionary<string, object>> saveList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(((JsonElement) saveListJson).GetRawText());

            foreach (Node node in Nodes)
            {
                node.Delete(true);
            }

            Nodes.Clear();

            foreach (Dictionary<string, object> nodeJsonData in saveList)
            {
                Node.GetNodeFromJsonData<Node>(nodeJsonData);
            }

            foreach (Node node in Nodes)
            {
                node.Display();
            }
        }
    }
}

namespace TTTReborn.Player
{
    using TTTReborn.UI.VisualProgramming;

    public partial class TTTPlayer
    {
        internal static Dictionary<string, object> LoadVisualProgramming(string path, string fileName, Utils.Realm realm)
        {
            path = Utils.GetSettingsFolderPath(realm, path);

            if (FileSystem.Data.FileExists(path + fileName + Window.VISUALPROGRAMMING_FILE_EXTENSION))
            {
                string jsonData = FileSystem.Data.ReadAllText(path + fileName + Window.VISUALPROGRAMMING_FILE_EXTENSION);

                if (jsonData == null || string.IsNullOrEmpty(jsonData))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);
            }

            return null;
        }
    }
}
