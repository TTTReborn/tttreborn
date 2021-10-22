using System;
using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class Dropdown : Panel
    {
        public readonly Label TextLabel;

        public Action<DropdownOption> OnSelectOption { get; set; }

        public bool IsDeleted { get; private set; } = false;

        public bool IsOpen
        {
            get => OptionHolder.Enabled;
            private set
            {
                OptionHolder.Enabled = value;

                _openLabel.SetClass("opened", value);
            }
        }

        private readonly Label _openLabel;

        public readonly List<DropdownOption> Options = new();

        public readonly DropdownOptionHolder OptionHolder;

        public DropdownOption SelectedOption
        {
            get => _selectedOption;
            private set
            {
                _selectedOption = value;

                TextLabel.Text = _selectedOption.TextLabel.Text;
            }
        }
        private DropdownOption _selectedOption;

        public Dropdown(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/components/dropdown/Dropdown.scss");

            TextLabel = Add.Label("Select...", "textLabel");
            _openLabel = Add.Label("expand_more", "openLabel");

            OptionHolder = new DropdownOptionHolder(this);

            IsOpen = false;
        }

        public DropdownOption AddOption(string text, object data = null, Action<Panel> onSelect = null)
        {
            DropdownOption dropdownOption = new DropdownOption(this, OptionHolder, text, data)
            {
                OnSelect = onSelect
            };

            Options.Add(dropdownOption);

            return dropdownOption;
        }

        public void SelectByName(string optionName)
        {
            foreach (DropdownOption option in Options)
            {
                if (option.TextLabel.Text.Equals(optionName))
                {
                    SelectedOption = option;

                    return;
                }
            }
        }

        public void SelectByData(object data)
        {
            foreach (DropdownOption option in Options)
            {
                if (option.Data.Equals(data))
                {
                    SelectedOption = option;

                    return;
                }
            }
        }

        public virtual void OnSelectDropdownOption(DropdownOption option)
        {
            SelectedOption = option;

            OnSelectOption?.Invoke(SelectedOption);

            IsOpen = false;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            base.OnClick(e);

            IsOpen = !IsOpen;
        }

        public override void OnDeleted()
        {
            base.OnDeleted();

            IsDeleted = true;
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class DropdownConstructor
    {
        public static Dropdown Dropdown(this PanelCreator self, string className = null)
        {
            Dropdown dropdown = new(self.panel);

            if (className != null)
            {
                dropdown.AddClass(className);
            }

            return dropdown;
        }
    }
}
