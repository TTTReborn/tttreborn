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
            Tabs tabs = tabContent.Add.Tabs();
            tabs.AddTab("Sprint", (panelContent) =>
            {
                AddSprintSettings(panelContent, serverSettings);
            }, "sprint");

            tabs.AddTab("Rounds", (panelContent) =>
            {
                AddRoundSettings(panelContent, serverSettings);
            }, "rounds");

            tabs.AddTab("AFK", (panelContent) =>
            {
                AddAFKSettings(panelContent, serverSettings);
            }, "afk");
        }

        private void AddAFKSettings(PanelContent panelContent, ServerSettings serverSettings)
        {
            // TTTMinPlayers
            CreateSettingsEntry(panelContent, "AFK Time", serverSettings.Round.MinPlayers, "The length of time in minutes before a player is marked AFK.", (value) =>
            {
                serverSettings.AFK.MinutesTillKick = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON(serverSettings, true));
            });
        }

        private void AddSprintSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            Sandbox.UI.Panel sprintPanel = tabContent.Add.Panel("sprint");
            sprintPanel.Add.Label($"Sprint enabled?");

            Switch sw = sprintPanel.Add.Switch("sprint", serverSettings.Movement.IsSprintEnabled);
            sw.AddEventListener("onchange", (panelEvent) =>
            {
                serverSettings.Movement.IsSprintEnabled = !serverSettings.Movement.IsSprintEnabled;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });
        }

        private void AddRoundSettings(PanelContent panelContent, ServerSettings serverSettings)
        {
            // TTTMinPlayers
            CreateSettingsEntry<int>(panelContent, "Min Players", serverSettings.Round.MinPlayers, "The minimum players required to start.", (value) =>
            {
                serverSettings.Round.MinPlayers = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TTTPreRoundTime
            CreateSettingsEntry<int>(panelContent, "PreRound Time", serverSettings.Round.PreRoundTime, "The amount of time allowed for preparation.", (value) =>
            {
                serverSettings.Round.PreRoundTime = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TTTRoundTime
            CreateSettingsEntry<int>(panelContent, "Round Time", serverSettings.Round.RoundTime, "The amount of time allowed for the main round.", (value) =>
            {
                serverSettings.Round.RoundTime = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TTTPostRoundTime
            CreateSettingsEntry<int>(panelContent, "PostRound Time", serverSettings.Round.PostRoundTime, "The amount of time before the next round starts.", (value) =>
            {
                serverSettings.Round.PostRoundTime = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TTTKillTimeReward
            CreateSettingsEntry<int>(panelContent, "Kill Time Reward", serverSettings.Round.KillTimeReward, "The amount of extra time given to traitors for killing an innocent.", (value) =>
            {
                serverSettings.Round.KillTimeReward = value;

                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });
        }

        private TextEntry CreateSettingsEntry<T>(Sandbox.UI.Panel parent, string title, T defaultValue, string description, Action<T> OnChange = null)
        {
            Sandbox.UI.Panel wrapper = parent.Add.Panel();
            Label textLabel = wrapper.Add.Label(title);
            textLabel.AddTooltip(description, "");

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
