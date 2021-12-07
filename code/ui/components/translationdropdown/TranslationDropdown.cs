using System;

using Sandbox.Html;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationDropdown : DropDown
    {
        public TranslationDropdown() : base() { }

        public new Option Selected
        {
            get => base.Selected;
            private set
            {
                base.Selected = value;
            }
        }

        public void Select(object value)
        {
            foreach (Option option in Options)
            {
                if (option.Value.Equals(value))
                {
                    Selected = option;
                }
            }
        }

        public void Select(Option newOption)
        {
            foreach (Option option in Options)
            {
                if (option.Title == newOption.Title || option.Value.Equals(newOption.Value))
                {
                    Selected = option;
                }
            }
        }

        public override bool OnTemplateElement(INode element)
        {
            Options.Clear();

            foreach (INode child in element.Children)
            {
                if (!child.IsElement)
                {
                    continue;
                }

                if (child.Name.Equals("translationoption", StringComparison.OrdinalIgnoreCase))
                {
                    Option o = new();

                    o.Title = child.GetAttribute("key");
                    o.Value = child.GetAttribute("value", o.Title);
                    o.Icon = child.GetAttribute("icon", null);

                    Options.Add(o);
                }
            }

            Select(Value);
            return true;
        }
    }

    public class TranslationOption : Option
    {
        public TranslationOption(TranslationData titleData, object data) : base(TTTLanguage.ActiveLanguage.GetFormattedTranslation(titleData), data) { }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TranslationDropdownConstructor
    {
        public static TranslationDropdown TranslationDropdown(this PanelCreator self)
        {
            TranslationDropdown translationDropdown = new();

            self.panel.AddChild(translationDropdown);

            return translationDropdown;
        }
    }
}
