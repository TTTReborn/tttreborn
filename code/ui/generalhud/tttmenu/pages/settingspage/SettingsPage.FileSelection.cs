using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class SettingsPage : Panel
    {
        public static void CreateFileSelectionButtons(Panel parent, FileSelection currentFileSelection, FileSelection serverSettingsFileSelection, bool isServerSettings)
        {
            Panel wrapper = new();

            wrapper.Add.TranslationButton(new TranslationData("MENU_SETTINGS_BUTTONS_SAVE"), null, null, () =>
            {
                currentFileSelection?.Close();

                FileSelection fileSelection = parent.FindPopupPanel().Add.FileSelection();
                fileSelection.DefaultSelectionFileType = $"*{SettingFunctions.SETTINGS_FILE_EXTENSION}";
                fileSelection.OnAgree = () => OnAgreeSaveAs(fileSelection, isServerSettings);
                fileSelection.DefaultSelectionPath = GetSettingsPathByData(isServerSettings);
                fileSelection.EnableFileNameEntry();
                fileSelection.Display();

                currentFileSelection = fileSelection;
            });

            wrapper.Add.TranslationButton(new TranslationData("MENU_SETTINGS_BUTTONS_LOAD"), null, null, () =>
            {
                currentFileSelection?.Close();

                FileSelection fileSelection = parent.FindPopupPanel().Add.FileSelection();
                fileSelection.DefaultSelectionFileType = $"*{SettingFunctions.SETTINGS_FILE_EXTENSION}";
                fileSelection.OnAgree = () => OnAgreeLoadFrom(fileSelection, isServerSettings);
                fileSelection.DefaultSelectionPath = GetSettingsPathByData(isServerSettings);

                if (isServerSettings)
                {
                    fileSelection.Header.NavigationHeader.OnClose = (modal) =>
                    {
                        if (serverSettingsFileSelection != fileSelection)
                        {
                            return;
                        }

                        serverSettingsFileSelection = null;
                    };

                    serverSettingsFileSelection = fileSelection;
                }
                else
                {
                    serverSettingsFileSelection = null;
                }

                fileSelection.Display();

                currentFileSelection = fileSelection;
            });

            parent.AddChild(wrapper);
        }

        private static void OnAgreeSaveAs(FileSelection fileSelection, bool isServerSettings)
        {
            string fileName = fileSelection.FileNameEntry.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            fileSelection.Close();

            fileName = fileName.Split('/')[^1].Split('.')[0];

            if (isServerSettings)
            {
                Player.TTTPlayer.RequestSaveServerSettingsAs(fileSelection.CurrentFolderPath, fileName);
            }
            else
            {
                if (!FileSystem.Data.FileExists(fileSelection.CurrentFolderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION))
                {
                    SettingFunctions.SaveSettings(ClientSettings.Instance, fileSelection.CurrentFolderPath, fileName);
                }
                else
                {
                    AskOverwriteSelectedSettings(fileSelection.CurrentFolderPath, fileName, () => SettingFunctions.SaveSettings(ClientSettings.Instance, fileSelection.CurrentFolderPath, fileName));
                }
            }
        }

        private static void OnAgreeLoadFrom(FileSelection fileSelection, bool isServerSettings)
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

            if (isServerSettings)
            {
                Player.TTTPlayer.RequestLoadFrom(fileSelection.CurrentFolderPath, fileName);
            }
            else
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
                // menuContent.SetPanelContent(OpenSettings);
            }
        }

        public static void AskOverwriteSelectedSettings(string folderPath, string fileName, Action onConfirm)
        {
            string fullFilePath = folderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION;

            DialogBox dialogBox = new();
            dialogBox.SetTranslationTitle(new TranslationData("MENU_SETTINGS_BUTTONS_SAVE_OVERWRITE", fullFilePath));
            dialogBox.AddTranslation(new TranslationData("MENU_SETTINGS_BUTTONS_SAVE_OVERWRITE_TEXT", fullFilePath));
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

        private static string GetSettingsPathByData(bool isServerSettings)
        {
            if (isServerSettings)
            {
                return $"/settings/{Utils.GetTypeName(typeof(ServerSettings)).ToLower()}/";
            }
            else
            {
                return $"/settings/{Utils.GetTypeName(typeof(ClientSettings)).ToLower()}/";
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
            SettingsPage.AskOverwriteSelectedSettings(filePath, fileName, () =>
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
            // Menu menu = Menu.Instance;

            // if (menu != null && menu.Enabled && menu.ServerSettingsTabContent != null)
            // {
            //     // refresh settings
            //     menu.Content.SetPanelContent(menu.OpenSettings);
            //     menu.SettingsTabs?.SelectByValue(Utils.Realm.Server);

            //     menu.ServerSettingsFileSelection?.Close();
            // }
        }
    }
}
