using Sandbox;
using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ClientSettingsPage : Panel
    {
        private Panel Root { get; set; }

        public ClientSettingsPage()
        {
            SettingsPage.CreateSettings(Root, ClientSettings.Instance);
        }
    }
}