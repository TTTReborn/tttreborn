using System;

namespace TTTReborn.Settings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SettingAttribute : Attribute
    {
        public bool AvoidTranslation { private set; get; }

        public SettingAttribute(bool avoidTranslation = false) : base()
        {
            AvoidTranslation = avoidTranslation;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InputSettingAttribute : SettingAttribute
    {
        public InputSettingAttribute(bool avoidTranslation = false) : base(avoidTranslation)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SwitchSettingAttribute : SettingAttribute
    {
        public SwitchSettingAttribute(bool avoidTranslation = false) : base(avoidTranslation)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DropdownSettingAttribute : SettingAttribute
    {
        public DropdownSettingAttribute(bool avoidTranslation = false) : base(avoidTranslation)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DropdownOptionsAttribute : SettingAttribute
    {
        public string DropdownSetting;

        public DropdownOptionsAttribute(string dropdownSetting, bool avoidTranslation = false) : base(avoidTranslation)
        {
            DropdownSetting = dropdownSetting;
        }
    }
}
