using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        internal void CreateServerSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            tabContent.DeleteChildren(true);

            AddSprintSettings(tabContent, serverSettings);

            AddRoundSettings(tabContent, serverSettings);
        }

        private void AddSprintSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            Panel sprintPanel = tabContent.Add.Panel("sprint");
            sprintPanel.Add.Label($"Sprint enabled?");

            Switch sw = sprintPanel.Add.Switch("sprint", serverSettings.IsSprintEnabled);
            sw.AddEventListener("onchange", (panelEvent) =>
            {
                serverSettings.IsSprintEnabled = !serverSettings.IsSprintEnabled;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });
        }

        private void AddRoundSettings(PanelContent panelContent, ServerSettings serverSettings)
        {
            // TTTMinPlayers
            CreateSettingsEntry<int>(panelContent, "Min Players", serverSettings.TTTMinPlayers, (value) =>
            {
                serverSettings.TTTMinPlayers = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TTTPreRoundTime
            CreateSettingsEntry<int>(panelContent, "PreRound Time", serverSettings.TTTPreRoundTime, (value) =>
            {
                serverSettings.TTTPreRoundTime = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TTTRoundTime
            CreateSettingsEntry<int>(panelContent, "Round Time", serverSettings.TTTRoundTime, (value) =>
            {
                serverSettings.TTTRoundTime = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TTTPostRoundTime
            CreateSettingsEntry<int>(panelContent, "PostRound Time", serverSettings.TTTPostRoundTime, (value) =>
            {
                serverSettings.TTTPostRoundTime = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TTTKillTimeReward
            CreateSettingsEntry<int>(panelContent, "Kill Time Reward", serverSettings.TTTKillTimeReward, (value) =>
            {
                serverSettings.TTTKillTimeReward = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });
        }

        private TextEntry CreateSettingsEntry<T>(Panel parent, string title, T defaultValue, Action<T> OnChange = null)
        {
            Panel wrapper = parent.Add.Panel();
            wrapper.Add.Label(title);

            TextEntry textEntry = wrapper.Add.TextEntry(defaultValue.ToString());

            textEntry.AddEventListener("onsubmit", (panelEvent) =>
            {
                try
                {
                    textEntry.Text.TryToType(typeof(T), out object value);

                    if (value.ToString().Equals(textEntry.Text))
                    {
                        T newValue = (T) value;

                        OnChange?.Invoke(newValue);

                        defaultValue = newValue;
                    }
                }
                catch (Exception) { }

                textEntry.Text = defaultValue.ToString();
            });

            return textEntry;
        }
    }
}
