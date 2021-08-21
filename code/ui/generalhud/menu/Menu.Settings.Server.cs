using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        internal void CreateServerSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            tabContent.DeleteChildren(true);

            tabContent.Add.Label($"Sprint enabled?");
            Switch sw = tabContent.Add.Switch("sprint", serverSettings.IsSprintEnabled);

            sw.AddEventListener("onchange", (panelEvent) =>
            {
                serverSettings.IsSprintEnabled = !serverSettings.IsSprintEnabled;

                // TODO Can be improved to avoid syncing EVERYTHING
                // send settings to server
                ConsoleSystem.Run("ttt_serversettings_send", SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });
        }
    }
}
