using System;
using System.Reflection;

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

            Type settingsType = serverSettings.GetType();
            PropertyInfo[] properties = settingsType.GetProperties();
            string nsp = typeof(Settings.Categories.Round).Namespace;

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.PropertyType.Namespace.Equals(nsp))
                {
                    string categoryName = propertyInfo.Name;
                    object propertyObject = propertyInfo.GetValue(serverSettings);

                    if (propertyObject == null)
                    {
                        continue;
                    }

                    foreach (PropertyInfo subPropertyInfo in propertyInfo.PropertyType.GetProperties())
                    {
                        foreach (object attribute in subPropertyInfo.GetCustomAttributes())
                        {
                            if (attribute is SettingAttribute settingAttribute)
                            {
                                string propertyName = subPropertyInfo.Name;

                                switch (settingAttribute)
                                {
                                    case SwitchSettingAttribute:
                                        SettingsPage.CreateSwitchSetting(this, serverSettings, categoryName, propertyName, propertyObject);

                                        break;

                                        // case InputSettingAttribute:
                                        //     CreateInputSetting(settings, panelContent, categoryName, propertyName, propertyObject);

                                        //     break;

                                        // case DropdownSettingAttribute:
                                        //     CreateDropdownSetting(settings, panelContent, categoryName, propertyName, propertyObject, propertyInfo, subPropertyInfo);

                                        //     break;
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
