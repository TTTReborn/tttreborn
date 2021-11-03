using System;

namespace TTTReborn.Settings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SettingAttribute : Attribute
    {
        public SettingAttribute() : base()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InputSettingAttribute : SettingAttribute
    {
        public InputSettingAttribute() : base()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SwitchSettingAttribute : SettingAttribute
    {
        public SwitchSettingAttribute() : base()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DropdownSettingAttribute : SettingAttribute
    {
        public DropdownSettingAttribute() : base()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DropdownOptionsAttribute : SettingAttribute
    {
        public string DropdownSetting;

        public DropdownOptionsAttribute(string dropdownSetting) : base()
        {
            DropdownSetting = dropdownSetting;
        }
    }
}
