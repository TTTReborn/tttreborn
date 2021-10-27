using Sandbox.UI.Construct;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        internal void CreateServerSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            Tabs tabs = tabContent.Add.Tabs();
            tabs.AddTab("MENU_SETTINGS_TAB_SPRINT", (panelContent) =>
            {
                AddSprintSettings(panelContent, serverSettings);
            }, "sprint");

            tabs.AddTab("MENU_SETTINGS_TAB_ROUND", (panelContent) =>
            {
                AddRoundSettings(panelContent, serverSettings);
            }, "rounds");

            tabs.AddTab("MENU_SETTINGS_TAB_AFK", (panelContent) =>
            {
                AddAFKSwitchSettings(panelContent, serverSettings);
                AddAFKSettings(panelContent, serverSettings);
            }, "afk");
        }

        private void AddAFKSettings(PanelContent panelContent, ServerSettings serverSettings)
        {
            // TTTMinPlayers
            CreateSettingsEntry(panelContent, "MENU_SETTINGS_AFK_TIME", serverSettings.AFK.SecondsTillKick, "MENU_SETTINGS_AFK_TIME_DESCRIPTION", (value) =>
            {
                serverSettings.AFK.SecondsTillKick = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });
        }

        private void AddAFKSwitchSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            Sandbox.UI.Panel sprintPanel = tabContent.Add.Panel("sprint");
            sprintPanel.Add.TranslationLabel("MENU_SETTINGS_KICK").AddTooltip("MENU_SETTINGS_KICK_DESCRIPTION");

            Switch sw = sprintPanel.Add.Switch("afk", serverSettings.AFK.ShouldKickPlayers);
            sw.AddEventListener("onchange", (panelEvent) =>
            {
                serverSettings.AFK.ShouldKickPlayers = !serverSettings.AFK.ShouldKickPlayers;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });
        }

        private void AddSprintSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            Sandbox.UI.Panel sprintPanel = tabContent.Add.Panel("sprint");
            sprintPanel.Add.TranslationLabel("MENU_SETTINGS_SPRINT").AddTooltip("MENU_SETTINGS_SPRINT_DESCRIPTION");

            Switch sw = sprintPanel.Add.Switch("sprint", serverSettings.Movement.IsSprintEnabled);
            sw.AddEventListener("onchange", (panelEvent) =>
            {
                serverSettings.Movement.IsSprintEnabled = !serverSettings.Movement.IsSprintEnabled;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });
        }

        private void AddRoundSettings(PanelContent panelContent, ServerSettings serverSettings)
        {
            // TTTMinPlayers
            CreateSettingsEntry(panelContent, "MENU_SETTINGS_MINPLAYERS", serverSettings.Round.MinPlayers, "MENU_SETTINGS_MINPLAYERS_DESCRIPTION", (value) =>
            {
                serverSettings.Round.MinPlayers = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });

            // TTTPreRoundTime
            CreateSettingsEntry(panelContent, "MENU_SETTINGS_PREROUND", serverSettings.Round.PreRoundTime, "MENU_SETTINGS_PREROUND_DESCRIPTION", (value) =>
            {
                serverSettings.Round.PreRoundTime = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });

            // TTTRoundTime
            CreateSettingsEntry(panelContent, "MENU_SETTINGS_ROUNDTIME", serverSettings.Round.RoundTime, "MENU_SETTINGS_ROUNDTIME_DESCRIPTION", (value) =>
            {
                serverSettings.Round.RoundTime = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });

            // TTTPostRoundTime
            CreateSettingsEntry(panelContent, "MENU_SETTINGS_POSTROUND", serverSettings.Round.PostRoundTime, "MENU_SETTINGS_POSTROUND_DESCRIPTION", (value) =>
            {
                serverSettings.Round.PostRoundTime = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });

            // TTTKillTimeReward
            CreateSettingsEntry(panelContent, "MENU_SETTINGS_KILLTIMEREWARD", serverSettings.Round.KillTimeReward, "MENU_SETTINGS_KILLTIMEREWARD_DESCRIPTION", (value) =>
            {
                serverSettings.Round.KillTimeReward = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });
        }
    }
}
