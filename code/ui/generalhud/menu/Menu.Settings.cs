namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                CreateSettingsButtons(panelContent);
            }, "Settings", "settings");
        }
    }
}
