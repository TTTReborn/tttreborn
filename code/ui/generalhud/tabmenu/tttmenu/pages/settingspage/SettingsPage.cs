using System;
using System.Collections.Generic;
using System.Reflection;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Settings;

#pragma warning disable CA1822

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class SettingsPage : Panel
    {
        private TranslationButton ServerSettingsButton { get; set; }

        public SettingsPage()
        {
            if (Local.Client.HasPermission("serversettings"))
            {
                ServerSettingsButton.RemoveClass("inactive");
            }
        }

        public void GoToClientSettings()
        {
            TTTMenu.Instance.AddPage(new ClientSettingsPage());
        }

        public void GoToServerSettings()
        {
            // Call to server which sends down server data and then adds the ServerSettingsPage.
            SettingFunctions.RequestServerSettings();
        }

        public static void CreateSettings(TranslationTabContainer tabContainer, Settings.Settings settings, Type settingsType = null)
        {
            settingsType ??= settings.GetType();

            Type baseSettingsType = typeof(Settings.Settings);

            if (settingsType != baseSettingsType)
            {
                CreateSettings(tabContainer, settings, settingsType.BaseType);
            }

            PropertyInfo[] properties = settingsType.GetProperties();
            string nsp = typeof(Settings.Categories.Round).Namespace;

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.DeclaringType.BaseType != baseSettingsType && settingsType != baseSettingsType || !propertyInfo.PropertyType.Namespace.Equals(nsp))
                {
                    continue;
                }

                string categoryName = propertyInfo.Name;
                object propertyObject = propertyInfo.GetValue(settings);

                if (propertyObject == null)
                {
                    continue;
                }

                Panel tab = new();
                tab.AddClass("root");

                foreach (PropertyInfo subPropertyInfo in propertyInfo.PropertyType.GetProperties())
                {
                    foreach (object attribute in subPropertyInfo.GetCustomAttributes())
                    {
                        if (attribute is not SettingAttribute settingAttribute)
                        {
                            continue;
                        }

                        string propertyName = subPropertyInfo.Name;

                        switch (settingAttribute)
                        {
                            case SwitchSettingAttribute:
                                CreateSwitchSetting(tab, settings, categoryName, propertyName, propertyObject);

                                break;

                            case InputSettingAttribute:
                                CreateInputSetting(tab, settings, categoryName, propertyName, propertyObject);

                                break;

                            case DropdownSettingAttribute:
                                CreateDropdownSetting(tab, settings, categoryName, propertyName, propertyObject, propertyInfo, subPropertyInfo);

                                break;
                        }

                        break;
                    }

                    tab.Add.LineBreak();
                }

                tabContainer.AddTab(tab, new TranslationData($"MENU.SETTINGS.TABS.{categoryName.ToUpper()}.TITLE"));
            }
        }

        private static void CreateSwitchSetting(Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject)
        {
            TranslationCheckbox checkbox = parent.Add.TranslationCheckbox(new TranslationData($"MENU.SETTINGS.TABS.{categoryName.ToUpper()}.{propertyName.ToUpper()}.TITLE"));
            checkbox.AddTooltip(new TranslationData($"MENU.SETTINGS.TABS.{categoryName.ToUpper()}.{propertyName.ToUpper()}.DESCRIPTION"));
            checkbox.Checked = Utils.GetPropertyValue<bool>(propertyObject, propertyName);
            checkbox.AddEventListener("onchange", (panelEvent) =>
            {
                UpdateSettingsProperty(settings, propertyObject, propertyName, checkbox.Checked);
            });
        }

        private static void CreateInputSetting(Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject)
        {
            CreateSettingsEntry(parent, $"MENU.SETTINGS.TABS.{categoryName.ToUpper()}.{propertyName.ToUpper()}.TITLE", Utils.GetPropertyValue(propertyObject, propertyName).ToString(), $"MENU.SETTINGS.TABS.{categoryName.ToUpper()}.{propertyName.ToUpper()}.DESCRIPTION", (value) =>
            {
                UpdateSettingsProperty(settings, propertyObject, propertyName, value);
            });
        }

        private static void CreateDropdownSetting(Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject, PropertyInfo propertyInfo, PropertyInfo subPropertyInfo)
        {
            parent.Add.Panel(categoryName.ToLower());
            parent.Add.TranslationLabel(new TranslationData($"MENU.SETTINGS.TABS.{categoryName.ToUpper()}.{propertyName.ToUpper()}.TITLE"), "h3").AddTooltip(new TranslationData($"MENU.SETTINGS.TABS.{categoryName.ToUpper()}.{propertyName.ToUpper()}.DESCRIPTION"));

            TranslationDropdown dropdownSelection = parent.Add.TranslationDropdown();

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
                            Option option;

                            if (dropdownOptionsAttribute.AvoidTranslation)
                            {
                                option = new Option(keyValuePair.Key, keyValuePair.Value);
                            }
                            else
                            {
                                option = new TranslationOption(new TranslationData(keyValuePair.Key), keyValuePair.Value);
                            }

                            dropdownSelection.Options.Add(option);
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
                GameEvent.Register(new Events.Settings.ChangeEvent());
            }
        }

        public static TextEntry CreateSettingsEntry<T>(Panel parent, string title, T defaultValue, string description, Action<T> OnSubmit = null, Action<T> OnChange = null)
        {
            TranslationLabel textLabel = parent.Add.TranslationLabel(new TranslationData(title));
            textLabel.AddTooltip(new TranslationData(description));

            TranslationTextEntry textEntry = parent.Add.TranslationTextEntry();
            textEntry.Text = defaultValue.ToString();

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
