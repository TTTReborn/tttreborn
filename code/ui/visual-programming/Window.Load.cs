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
using System.Text.Json;

using Sandbox;
using Sandbox.UI.Construct;

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

            if (Instance == null)
            {
                fileSelection.Close();

                return;
            }

            Dictionary<string, object> jsonData = Player.TTTPlayer.LoadVisualProgramming(fileSelection.CurrentFolderPath, fileName + VISUALPROGRAMMING_FILE_EXTENSION, Utils.Realm.Client);

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
                if (node is MainNode mainNode)
                {
                    MainNode = mainNode;
                }

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

            if (FileSystem.Data.FileExists(path + fileName))
            {
                string jsonData = FileSystem.Data.ReadAllText(path + fileName);

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
