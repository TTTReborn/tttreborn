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
                AddAFKSwitchSettings(panelContent, serverSettings);
                AddAFKSettings(panelContent, serverSettings);
            }, "afk");
        }

        private void AddAFKSettings(PanelContent panelContent, ServerSettings serverSettings)
        {
            // TTTMinPlayers
            CreateSettingsEntry(panelContent, "AFK Time", serverSettings.AFK.SecondsTillKick, "The length of time in minutes before a player is marked AFK.", (value) =>
            {
                serverSettings.AFK.SecondsTillKick = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });
        }

        private void AddAFKSwitchSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            Sandbox.UI.Panel sprintPanel = tabContent.Add.Panel("sprint");
            sprintPanel.Add.Label($"Should Kick Players?").AddTooltip("Whether or not a player should be kicked or moved to Spectators.");

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
            sprintPanel.Add.Label($"Sprint enabled?").AddTooltip("Whether or not sprint should be enabled on the server.");

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
            CreateSettingsEntry(panelContent, "Min Players", serverSettings.Round.MinPlayers, "The minimum players required to start.", (value) =>
            {
                serverSettings.Round.MinPlayers = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });

            // TTTPreRoundTime
            CreateSettingsEntry(panelContent, "PreRound Time", serverSettings.Round.PreRoundTime, "The amount of time allowed for preparation.", (value) =>
            {
                serverSettings.Round.PreRoundTime = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });

            // TTTRoundTime
            CreateSettingsEntry(panelContent, "Round Time", serverSettings.Round.RoundTime, "The amount of time allowed for the main round.", (value) =>
            {
                serverSettings.Round.RoundTime = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });

            // TTTPostRoundTime
            CreateSettingsEntry(panelContent, "PostRound Time", serverSettings.Round.PostRoundTime, "The amount of time before the next round starts.", (value) =>
            {
                serverSettings.Round.PostRoundTime = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });

            // TTTKillTimeReward
            CreateSettingsEntry(panelContent, "Kill Time Reward", serverSettings.Round.KillTimeReward, "The amount of extra time given to traitors for killing an innocent.", (value) =>
            {
                serverSettings.Round.KillTimeReward = value;

                SettingFunctions.SendSettingsToServer(serverSettings);
            });
        }
    }
}
