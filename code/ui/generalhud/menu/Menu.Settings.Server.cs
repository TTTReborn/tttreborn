using System;
using System.Collections.Generic;
using System.Reflection;

using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        internal void CreateServerSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            Tabs tabs = tabContent.Add.Tabs();

            Dictionary<string, Dictionary<string, SettingAttribute>> serverSettingsData = new();
            PropertyInfo[] properties = serverSettings.GetType().GetProperties();

            Type baseSettingsType = typeof(Settings.Settings);
            string nsp = typeof(Settings.Categories.Round).Namespace;

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.DeclaringType.BaseType == baseSettingsType && propertyInfo.PropertyType.Namespace.Equals(nsp))
                {
                    string categoryName = propertyInfo.Name;
                    object propertyObject = propertyInfo.GetValue(serverSettings);

                    if (propertyObject == null)
                    {
                        continue;
                    }

                    tabs.AddTab($"MENU_SETTINGS_TAB_{categoryName.ToUpper()}", (panelContent) =>
                    {
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
                                            Sandbox.UI.Panel uiPanel = panelContent.Add.Panel(categoryName.ToLower());
                                            uiPanel.Add.TranslationLabel($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}").AddTooltip($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION");

                                            Switch sw = uiPanel.Add.Switch(propertyName.ToLower(), Utils.GetPropertyValue<bool>(propertyObject, propertyName));
                                            sw.AddEventListener("onchange", (panelEvent) =>
                                            {
                                                Utils.SetPropertyValue(propertyObject, propertyName, !Utils.GetPropertyValue<bool>(propertyObject, propertyName));

                                                SettingFunctions.SendSettingsToServer(serverSettings);
                                            });

                                            break;

                                        case InputSettingAttribute:
                                            CreateSettingsEntry(panelContent, $"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}", Utils.GetPropertyValue(propertyObject, propertyName).ToString(), $"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION", (value) =>
                                            {
                                                Utils.SetPropertyValue(propertyObject, propertyName, value);

                                                SettingFunctions.SendSettingsToServer(serverSettings);
                                            });

                                            break;

                                        case DropdownSettingAttribute:


                                            break;
                                    }

                                    break;
                                }
                            }
                        }
                    }, categoryName.ToLower());
                }
            }
        }
    }
}
