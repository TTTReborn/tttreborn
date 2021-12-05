using System;
using System.Collections.Generic;
using System.Reflection;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
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

                    tabs.AddTab(new TranslationData($"MENU_SETTINGS_TAB_{categoryName.ToUpper()}"), (panelContent) =>
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
            TranslationCheckbox checkbox = panelContent.Add.TranslationCheckbox(new TranslationData($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}"));
            checkbox.AddTooltip(new TranslationData($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION"));
            checkbox.Checked = Utils.GetPropertyValue<bool>(propertyObject, propertyName);
            checkbox.AddEventListener("onchange", (panelEvent) =>
            {
                UpdateSettingsProperty(settings, propertyObject, propertyName, checkbox.Checked);
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
            uiPanel.Add.TranslationLabel(new TranslationData($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}")).AddTooltip(new TranslationData($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION"));

            TranslationDropdown dropdownSelection = uiPanel.Add.TranslationDropdown();

            foreach (PropertyInfo possibleDropdownPropertyInfo in propertyInfo.PropertyType.GetProperties())
            {
                foreach (object possibleDropdownAttribute in possibleDropdownPropertyInfo.GetCustomAttributes())
                {
                    if (possibleDropdownAttribute is DropdownOptionsAttribute dropdownOptionsAttribute && dropdownOptionsAttribute.DropdownSetting.Equals(subPropertyInfo.Name))
                    {
                        dropdownSelection.AddEventListener("onchange", (e) =>
                        {
                            UpdateSettingsProperty(settings, propertyObject, propertyName, dropdownSelection.Selected.Value);
                        });

                        foreach (KeyValuePair<string, object> keyValuePair in Utils.GetPropertyValue<Dictionary<string, object>>(propertyObject, possibleDropdownPropertyInfo.Name))
                        {
                            dropdownSelection.Options.Add(new TranslationOption(new TranslationData(keyValuePair.Key), keyValuePair.Value));
                        }
                    }
                }
            }

            dropdownSelection.Select(Utils.GetPropertyValue<string>(propertyObject, propertyName));
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
                Event.Run(Events.TTTEvent.Settings.Change);
            }
        }
    }
}
