namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                // add separation between server and client

                // client
                // add language

                // server (send / receive via json)
                // add sprint

                CreateSettingsButtons(panelContent);
            }, "Settings", "settings");
        }
    }
}
