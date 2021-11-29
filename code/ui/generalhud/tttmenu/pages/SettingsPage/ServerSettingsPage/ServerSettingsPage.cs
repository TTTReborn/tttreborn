using Sandbox;
using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ServerSettingsPage : Panel
    {
        private ServerSettings _serverSettings;

        public ServerSettingsPage(ServerSettings serverSettings)
        {
            _serverSettings = serverSettings;
        }
    }
}
