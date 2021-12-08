using Sandbox;
using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ServerSettingsPage : Panel
    {
        private Panel Root { get; set; }

        public ServerSettingsPage(ServerSettings serverSettings)
        {
            SettingsPage.CreateSettings(Root, serverSettings);
        }
    }
}