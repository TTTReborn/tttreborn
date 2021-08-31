using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class Tab : TTTPanel
    {
        public readonly Label TitleLabel;

        public Action OnSelectTab { get; set; }
        public Action<PanelContent> CreateContent { get; set; }

        public object Value { get; set; }

        private readonly Tabs _parentTabs;

        public Tab(Panel parent, Tabs parentTabs, string title, Action<PanelContent> createContent, object value = null, Action onSelectTab = null) : base(parent)
        {
            Parent = parent;
            _parentTabs = parentTabs;
            CreateContent = createContent;
            Value = value;
            OnSelectTab = onSelectTab;

            TitleLabel = Add.Label(title, "title");
        }

        protected override void OnClick(MousePanelEvent e)
        {
            _parentTabs.OnSelectTab(this);
        }
    }
}
