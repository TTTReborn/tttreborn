// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;
using System.Collections.Generic;
using System.Reflection;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Events;
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
                                            CreateSwitchSetting(settings, panelContent, categoryName, propertyName, propertyObject);

                                            break;

                                        case InputSettingAttribute:
                                            CreateInputSetting(settings, panelContent, categoryName, propertyName, propertyObject);

                                            break;

                                        case DropdownSettingAttribute:
                                            CreateDropdownSetting(settings, panelContent, categoryName, propertyName, propertyObject, propertyInfo, subPropertyInfo);

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

        private static void CreateSwitchSetting(Settings.Settings settings, PanelContent panelContent, string categoryName, string propertyName, object propertyObject)
        {
            Sandbox.UI.Panel uiPanel = panelContent.Add.Panel(categoryName.ToLower());
            uiPanel.Add.TranslationLabel($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}").AddTooltip($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION");

            Switch sw = uiPanel.Add.Switch(propertyName.ToLower(), Utils.GetPropertyValue<bool>(propertyObject, propertyName));
            sw.AddEventListener("onchange", (panelEvent) =>
            {
                UpdateSettingsProperty(settings, propertyObject, propertyName, sw.Checked);
            });
        }

        private static void CreateInputSetting(Settings.Settings settings, PanelContent panelContent, string categoryName, string propertyName, object propertyObject)
        {
            CreateSettingsEntry(panelContent, $"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}", Utils.GetPropertyValue(propertyObject, propertyName).ToString(), $"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION", (value) =>
            {
                UpdateSettingsProperty(settings, propertyObject, propertyName, value);
            });
        }

        private static void CreateDropdownSetting(Settings.Settings settings, PanelContent panelContent, string categoryName, string propertyName, object propertyObject, PropertyInfo propertyInfo, PropertyInfo subPropertyInfo)
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
                            UpdateSettingsProperty(settings, propertyObject, propertyName, (string) option.Data);
                        };
                    }
                }
            }

            dropdownSelection.SelectByData(Utils.GetPropertyValue<string>(propertyObject, propertyName));
        }

        private static void UpdateSettingsProperty<T>(Settings.Settings settings, object propertyObject, string propertyName, T value)
        {
            Utils.SetPropertyValue(propertyObject, propertyName, value);

            if (Gamemode.Game.Instance.Debug)
            {
                Log.Debug($"Set {propertyName} to {value}");
            }

            if (settings is ServerSettings serverSettings)
            {
                SettingFunctions.SendSettingsToServer(serverSettings);
            }
            else
            {
                Event.Run(TTTEvent.Settings.Change);
            }
        }
    }
}
