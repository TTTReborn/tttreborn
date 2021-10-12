using System;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    using Sandbox.UI.Construct;

    using TTTReborn.Globals;

    public partial class Menu
    {
        public PanelContent ServerSettingsTabContent { get; private set; }

        public Tabs SettingsTabs { get; private set; }

        internal void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((menuContent) =>
            {
                SettingsTabs = menuContent.Add.Tabs();
                SettingsTabs.AddTab("Client", CreateClientSettings, Utils.Realm.Client);

                if (Local.Client.HasPermission("serversettings"))
                {
                    SettingsTabs.AddTab("Server", InitServerSettings, Utils.Realm.Server);
                }

                CreateSettingsButtons(menuContent);
            }, "Settings", "settings");
        }

        private void InitServerSettings(PanelContent tabContent)
        {
            ServerSettingsTabContent = tabContent;

            tabContent.Add.Label("Loading...");

            SettingFunctions.RequestServerSettings();
        }

        internal void ProceedServerSettings(ServerSettings serverSettings)
        {
            if (!Enabled || ServerSettingsTabContent == null)
            {
                return;
            }

            ServerSettingsTabContent.SetPanelContent((menuContent) => CreateServerSettings(menuContent, serverSettings));
        }

        internal static TextEntry CreateSettingsEntry<T>(Sandbox.UI.Panel parent, string title, T defaultValue, string description, Action<T> OnSubmit = null, Action<T> OnChange = null)
        {
            Sandbox.UI.Panel wrapper = parent.Add.Panel();
            Label textLabel = wrapper.Add.Label(title);
            textLabel.AddTooltip(description, "");

            TextEntry textEntry = wrapper.Add.TextEntry(defaultValue.ToString());
            textEntry.AddClass("setting");
            textEntry.AddClass("rounded");
            textEntry.AddClass("box-shadow");
            textEntry.AddClass("background-color-secondary");

            textEntry.AddEventListener("onsubmit", (panelEvent) =>
            {
                try
                {
                    textEntry.Text.TryToType(typeof(T), out object value);

                    if (value.ToString().Equals(textEntry.Text))
                    {
                        T newValue = (T) value;

                        OnSubmit?.Invoke(newValue);

                        defaultValue = newValue;
                    }
                }
                catch (Exception) { }

                textEntry.Text = defaultValue.ToString();
            });

            textEntry.AddEventListener("onchange", (panelEvent) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(textEntry.Text))
                    {
                        return;
                    }

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
