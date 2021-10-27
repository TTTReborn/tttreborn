using System;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        public FileSelection ServerSettingsFileSelection { get; private set; }

        private FileSelection _currentFileSelection;

        private string GetSettingsPathByData(object data)
        {
            if (data is Utils.Realm realm)
            {
                if (realm == Utils.Realm.Client)
                {
                    return $"/settings/{Utils.GetTypeName(typeof(ClientSettings)).ToLower()}/";
                }
                else
                {
                    return $"/settings/{Utils.GetTypeName(typeof(ServerSettings)).ToLower()}/";
                }
            }

            return $"/settings/{Utils.GetTypeName(SettingsManager.Instance.GetType()).ToLower()}/";
        }

        private void CreateSettingsButtons(PanelContent menuContent)
        {
            Sandbox.UI.Panel buttonsWrapperPanel = menuContent.Add.Panel("wrapper");

            buttonsWrapperPanel.Add.TranslationButton("MENU_SETTINGS_BUTTONS_SAVE", "fileselectionbutton", () =>
            {
                _currentFileSelection?.Close();

                FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                fileSelection.DefaultSelectionFileType = $"*{SettingFunctions.SETTINGS_FILE_EXTENSION}";
                fileSelection.OnAgree = () => OnAgreeSaveAs(fileSelection);
                fileSelection.DefaultSelectionPath = GetSettingsPathByData(SettingsTabs.SelectedTab.Value);
                fileSelection.EnableFileNameEntry();
                fileSelection.Display();

                _currentFileSelection = fileSelection;
            });

            buttonsWrapperPanel.Add.TranslationButton("MENU_SETTINGS_BUTTONS_LOAD", "fileselectionbutton", () =>
            {
                _currentFileSelection?.Close();

                FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                fileSelection.DefaultSelectionFileType = $"*{SettingFunctions.SETTINGS_FILE_EXTENSION}";
                fileSelection.OnAgree = () => OnAgreeLoadFrom(fileSelection, menuContent);
                fileSelection.DefaultSelectionPath = GetSettingsPathByData(SettingsTabs.SelectedTab.Value);

                if (SettingsTabs.SelectedTab.Value is Utils.Realm realm && realm == Utils.Realm.Server)
                {
                    fileSelection.Header.NavigationHeader.OnClose = (modal) =>
                    {
                        if (ServerSettingsFileSelection != fileSelection)
                        {
                            return;
                        }

                        ServerSettingsFileSelection = null;
                    };

                    ServerSettingsFileSelection = fileSelection;
                }
                else
                {
                    ServerSettingsFileSelection = null;
                }

                fileSelection.Display();

                _currentFileSelection = fileSelection;
            });
        }

        private void OnAgreeSaveAs(FileSelection fileSelection)
        {
            string fileName = fileSelection.FileNameEntry.Text;

            if (string.IsNullOrEmpty(fileName) || SettingsTabs == null)
            {
                return;
            }

            fileSelection.Close();

            fileName = fileName.Split('/')[^1].Split('.')[0];

            if (SettingsTabs.SelectedTab.Value is not Utils.Realm realm)
            {
                return;
            }

            if (realm == Utils.Realm.Client)
            {
                if (!FileSystem.Data.FileExists(fileSelection.CurrentFolderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION))
                {
                    SettingFunctions.SaveSettings<ClientSettings>(ClientSettings.Instance, fileSelection.CurrentFolderPath, fileName);
                }
                else
                {
                    AskOverwriteSelectedSettings(fileSelection.CurrentFolderPath, fileName, () => SettingFunctions.SaveSettings<ClientSettings>(ClientSettings.Instance, fileSelection.CurrentFolderPath, fileName));
                }
            }
            else if (realm == Utils.Realm.Server)
            {
                Player.TTTPlayer.RequestSaveServerSettingsAs(fileSelection.CurrentFolderPath, fileName);
            }
        }

        internal static void AskOverwriteSelectedSettings(string folderPath, string fileName, Action onConfirm)
        {
            string fullFilePath = folderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION;

            DialogBox dialogBox = new DialogBox();
            dialogBox.SetTranslationTitle("MENU_SETTINGS_BUTTONS_SAVE_OVERWRITE", fullFilePath);
            dialogBox.AddTranslationText("MENU_SETTINGS_BUTTONS_SAVE_OVERWRITE_TEXT", fullFilePath);
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

        private void OnAgreeLoadFrom(FileSelection fileSelection, PanelContent menuContent)
        {
            if (fileSelection.SelectedEntry == null)
            {
                return;
            }

            string fileName = fileSelection.SelectedEntry.FileNameLabel.Text;

            if (string.IsNullOrEmpty(fileName) || SettingsTabs == null)
            {
                return;
            }

            fileName = fileName.Split('/')[^1].Split('.')[0];

            if (SettingsTabs.SelectedTab.Value is not Utils.Realm realm)
            {
                return;
            }

            if (realm == Utils.Realm.Client)
            {
                SettingsManager.Instance = SettingFunctions.LoadSettings<ClientSettings>(fileSelection.CurrentFolderPath, fileName);

                if (SettingsManager.Instance.LoadingError != SettingsLoadingError.None)
                {
                    Log.Error($"Settings file '{fileSelection.CurrentFolderPath}{fileName}{SettingFunctions.SETTINGS_FILE_EXTENSION}' can't be loaded. Reason: '{SettingsManager.Instance.LoadingError.ToString()}'");

                    return;
                }

                SettingFunctions.SaveSettings<ClientSettings>(ClientSettings.Instance);

                fileSelection.Close();

                // refresh settings
                menuContent.SetPanelContent(OpenSettings);
            }
            else if (realm == Utils.Realm.Server)
            {
                Player.TTTPlayer.RequestLoadFrom(fileSelection.CurrentFolderPath, fileName);
            }
        }
    }
}

namespace TTTReborn.Player
{
    using TTTReborn.UI.Menu;

    public partial class TTTPlayer
    {
        [ServerCmd(Name = "ttt_serversettings_saveas_request")]
        public static void RequestSaveServerSettingsAs(string filePath, string fileName, bool overwrite = false)
        {
            if (!ConsoleSystem.Caller.HasPermission("serversettings"))
            {
                return;
            }

            if (overwrite || !FileSystem.Data.FileExists(filePath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION))
            {
                SettingFunctions.SaveSettings<ServerSettings>(ServerSettings.Instance, filePath, fileName);
            }
            else
            {
                ClientAskOverwriteSelectedSettings(To.Single(ConsoleSystem.Caller), filePath, fileName);
            }
        }

        [ClientRpc]
        public static void ClientAskOverwriteSelectedSettings(string filePath, string fileName)
        {
            Menu.AskOverwriteSelectedSettings(filePath, fileName, () =>
            {
                RequestSaveServerSettingsAs(filePath, fileName, true);
            });
        }

        [ServerCmd(Name = "ttt_serversettings_loadfrom_request")]
        public static void RequestLoadFrom(string filePath, string fileName)
        {
            if (!ConsoleSystem.Caller.HasPermission("serversettings"))
            {
                return;
            }

            SettingsManager.Instance = SettingFunctions.LoadSettings<ServerSettings>(filePath, fileName);

            if (SettingsManager.Instance.LoadingError != SettingsLoadingError.None)
            {
                Log.Error($"Settings file '{filePath}{fileName}{SettingFunctions.SETTINGS_FILE_EXTENSION}' can't be loaded. Reason: '{SettingsManager.Instance.LoadingError.ToString()}'");

                return;
            }

            SettingFunctions.SaveSettings<ServerSettings>(ServerSettings.Instance);

            ClientFinishServerSettingsLoading(To.Single(ConsoleSystem.Caller), filePath, fileName);
        }

        [ClientRpc]
        public static void ClientFinishServerSettingsLoading(string filePath, string fileName)
        {
            Menu menu = Menu.Instance;

            if (menu != null && menu.Enabled && menu.ServerSettingsTabContent != null)
            {
                // refresh settings
                menu.Content.SetPanelContent(menu.OpenSettings);
                menu.SettingsTabs?.SelectByValue(Utils.Realm.Server);

                menu.ServerSettingsFileSelection?.Close();
            }
        }
    }
}
