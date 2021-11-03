using System;
using System.Collections.Generic;
using System.Reflection;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        internal void CreateSettings(Tabs tabs, Settings.Settings settings, Type settingsType = null)
        {
            settingsType ??= settings.GetType();

            Type baseSettingsType = typeof(Settings.Settings);

            if (settingsType != baseSettingsType)
            {
                CreateSettings(tabs, settings, settingsType.BaseType);
            }

            PropertyInfo[] properties = settingsType.GetProperties();
            string nsp = typeof(Settings.Categories.Round).Namespace;

            foreach (PropertyInfo propertyInfo in properties)
            {
                if ((propertyInfo.DeclaringType.BaseType == baseSettingsType || settingsType == baseSettingsType) && propertyInfo.PropertyType.Namespace.Equals(nsp))
                {
                    string categoryName = propertyInfo.Name;
                    object propertyObject = propertyInfo.GetValue(settings);

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
                                            {
                                                Sandbox.UI.Panel uiPanel = panelContent.Add.Panel(categoryName.ToLower());
                                                uiPanel.Add.TranslationLabel($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}").AddTooltip($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION");

                                                Switch sw = uiPanel.Add.Switch(propertyName.ToLower(), Utils.GetPropertyValue<bool>(propertyObject, propertyName));
                                                sw.AddEventListener("onchange", (panelEvent) =>
                                                {
                                                    Utils.SetPropertyValue(propertyObject, propertyName, !Utils.GetPropertyValue<bool>(propertyObject, propertyName));

                                                    if (settings is ServerSettings serverSettings)
                                                    {
                                                        SettingFunctions.SendSettingsToServer(serverSettings);
                                                    }
                                                    else
                                                    {
                                                        Event.Run(TTTEvent.Settings.Change);
                                                    }
                                                });
                                            }

                                            break;

                                        case InputSettingAttribute:
                                            {
                                                CreateSettingsEntry(panelContent, $"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}", Utils.GetPropertyValue(propertyObject, propertyName).ToString(), $"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION", (value) =>
                                                {
                                                    Utils.SetPropertyValue(propertyObject, propertyName, value);

                                                    if (settings is ServerSettings serverSettings)
                                                    {
                                                        SettingFunctions.SendSettingsToServer(serverSettings);
                                                    }
                                                    else
                                                    {
                                                        Event.Run(TTTEvent.Settings.Change);
                                                    }
                                                });
                                            }

                                            break;

                                        case DropdownSettingAttribute:
                                            {
                                                Sandbox.UI.Panel uiPanel = panelContent.Add.Panel(categoryName.ToLower());
                                                uiPanel.Add.TranslationLabel($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}").AddTooltip($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION");

                                                Dropdown dropdownSelection = uiPanel.Add.Dropdown(propertyName.ToLower());

                                                foreach (PropertyInfo possibleDropdownPropertyInfo in propertyInfo.PropertyType.GetProperties())
                                                {
                                                    foreach (object possibleDropdownAttribute in possibleDropdownPropertyInfo.GetCustomAttributes())
                                                    {
                                                        if (possibleDropdownAttribute is DropdownOptionsAttribute dropdownOptionsAttribute && dropdownOptionsAttribute.DropdownSetting.Equals(subPropertyInfo.Name))
                                                        {
                                                            foreach (KeyValuePair<string, object> keyValuePair in Utils.GetPropertyValue<Dictionary<string, object>>(propertyObject, possibleDropdownPropertyInfo.Name))
                                                            {
                                                                dropdownSelection.AddOption(keyValuePair.Key, keyValuePair.Value);
                                                            }

                                                            dropdownSelection.OnSelectOption = (option) =>
                                                            {
                                                                Utils.SetPropertyValue(propertyObject, propertyName, (string) option.Data);

                                                                if (settings is ServerSettings serverSettings)
                                                                {
                                                                    SettingFunctions.SendSettingsToServer(serverSettings);
                                                                }
                                                                else
                                                                {
                                                                    Event.Run(TTTEvent.Settings.Change);
                                                                }
                                                            };
                                                        }
                                                    }
                                                }

                                                dropdownSelection.SelectByData(Utils.GetPropertyValue<string>(propertyObject, propertyName));
                                            }

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
