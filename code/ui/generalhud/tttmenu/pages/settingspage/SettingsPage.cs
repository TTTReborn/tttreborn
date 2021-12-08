using System;
using System.Collections.Generic;
using System.Reflection;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Settings;

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

        public static void CreateSettings(Panel parent, Settings.Settings settings, Type settingsType = null)
        {
            settingsType ??= settings.GetType();

            Type baseSettingsType = typeof(Settings.Settings);

            if (settingsType != baseSettingsType)
            {
                CreateSettings(parent, settings, settingsType.BaseType);
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

                parent.Add.TranslationLabel(new TranslationData($"MENU_SETTINGS_TAB_{categoryName.ToUpper()}"), "h1");

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
                                CreateSwitchSetting(parent, settings, categoryName, propertyName, propertyObject);

                                break;

                            case InputSettingAttribute:
                                CreateInputSetting(parent, settings, categoryName, propertyName, propertyObject);

                                break;

                            case DropdownSettingAttribute:
                                CreateDropdownSetting(parent, settings, categoryName, propertyName, propertyObject, propertyInfo, subPropertyInfo);

                                break;
                        }

                        break;
                    }
                }
            }
        }

        private static void CreateSwitchSetting(Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject)
        {
            TranslationCheckbox checkbox = parent.Add.TranslationCheckbox(new TranslationData($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}"));
            checkbox.AddTooltip(new TranslationData($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION"));
            checkbox.Checked = Utils.GetPropertyValue<bool>(propertyObject, propertyName);
            checkbox.AddEventListener("onchange", (panelEvent) =>
            {
                UpdateSettingsProperty(settings, propertyObject, propertyName, checkbox.Checked);
            });
        }

        private static void CreateInputSetting(Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject)
        {
            CreateSettingsEntry(parent, $"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}", Utils.GetPropertyValue(propertyObject, propertyName).ToString(), $"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION", (value) =>
            {
                UpdateSettingsProperty(settings, propertyObject, propertyName, value);
            });
        }

        private static void CreateDropdownSetting(Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject, PropertyInfo propertyInfo, PropertyInfo subPropertyInfo)
        {
            parent.Add.Panel(categoryName.ToLower());
            parent.Add.TranslationLabel(new TranslationData($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}"), "h2").AddTooltip(new TranslationData($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}_DESCRIPTION"));

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

        // MZEGAR TODO Let's create a component for this.
        internal static TextEntry CreateSettingsEntry<T>(Sandbox.UI.Panel parent, string title, T defaultValue, string description, Action<T> OnSubmit = null, Action<T> OnChange = null, params object[] translationData)
        {
            Sandbox.UI.Panel wrapper = parent.Add.Panel();
            TranslationLabel textLabel = wrapper.Add.TranslationLabel(new TranslationData(title));
            textLabel.AddTooltip(new TranslationData(description), null, null, null, null);

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