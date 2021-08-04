using System;
using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class Dropdown : TTTPanel
    {
        public readonly Label TextLabel;

        public bool IsOpened
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

        public Dropdown(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/dropdown/Dropdown.scss");

            TextLabel = Add.Label("", "textLabel");
            _openLabel = Add.Label("expand_more", "openLabel");

            OptionHolder = new TTTPanel(this);
            OptionHolder.AddClass("optionholder");
        }

        public void AddOption(string text, Action<TTTPanel> onSelect)
        {
            Options.Add(new DropdownOption(OptionHolder, text)
            {
                Dropdown = this,
                OnSelect = onSelect
            });
        }

        public virtual void OnSelectOption(DropdownOption option)
        {
            IsOpened = false;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            IsOpened = !IsOpened;
        }
    }
}
