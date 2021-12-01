using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public partial class DropdownOption : Panel
    {
        public readonly Dropdown Dropdown;

        public readonly TranslationLabel TextLabel;

        public object Data { get; set; }

        public Action<Panel> OnSelect { get; set; }

        public DropdownOption(Dropdown dropdown, Sandbox.UI.Panel parent, TranslationData translationData, object data = null) : base(parent)
        {
            Dropdown = dropdown;
            TextLabel = Add.TranslationLabel(translationData, "optiontext");
            Data = data;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            OnSelect?.Invoke(this);
            Dropdown.OnSelectDropdownOption(this);
        }
    }
}
