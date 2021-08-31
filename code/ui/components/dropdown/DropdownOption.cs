using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class DropdownOption : TTTPanel
    {
        public readonly Dropdown Dropdown;

        public readonly Label TextLabel;

        public object Data { get; set; }

        public Action<TTTPanel> OnSelect { get; set; }

        public DropdownOption(Dropdown dropdown, Panel parent = null, string text = "", object data = null) : base(parent)
        {
            Parent = parent ?? Parent;

            Dropdown = dropdown;
            TextLabel = Add.Label(text, "optiontext");
            Data = data;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            OnSelect?.Invoke(this);
            Dropdown.OnSelectDropdownOption(this);
        }
    }
}
