using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class Tab : TTTPanel
    {
        public readonly Label TitleLabel;
        public readonly Tabs Tabs;

        public Action OnSelectTab { get; set; }
        public Action<Panel> Content { get; set; }

        public Tab(Panel parent, Tabs tabs) : base(parent)
        {
            Parent = parent;
            Tabs = tabs;

            TitleLabel = Add.Label("", "title");
        }

        public void SetTitle(string title)
        {
            TitleLabel.Text = title;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            Tabs.OnSelectTab(this);
        }
    }
}
