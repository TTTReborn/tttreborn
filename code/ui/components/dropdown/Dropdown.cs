using System;
using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class Dropdown : TTTPanel
    {
        public readonly Label TextLabel;

        public bool IsOpen
        {
            get => OptionHolder.IsShowing;
            private set
            {
                OptionHolder.IsShowing = value;

                _openLabel.SetClass("opened", value);
            }
        }

        private readonly Label _openLabel;

        public readonly List<DropdownOption> Options = new();

        public readonly TTTPanel OptionHolder;

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

        public Dropdown(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/dropdown/Dropdown.scss");

            TextLabel = Add.Label("Select...", "textLabel");
            _openLabel = Add.Label("expand_more", "openLabel");

            OptionHolder = new TTTPanel(this);
            OptionHolder.AddClass("optionholder");

            IsOpen = false;
        }

        public DropdownOption AddOption(string text, Action<TTTPanel> onSelect = null)
        {
            DropdownOption dropdownOption = new DropdownOption(OptionHolder, text)
            {
                Dropdown = this,
                OnSelect = onSelect
            };

            Options.Add(dropdownOption);

            return dropdownOption;
        }

        public virtual void OnSelectOption(DropdownOption option)
        {
            SelectedOption = option;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            IsOpen = !IsOpen;
        }
    }
}
