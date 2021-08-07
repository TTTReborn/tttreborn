using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class PanelHeader : TTTPanel
    {
        public Action<PanelHeader> OnClose { get; set; }

        private Label _title;

        private Button _closeButton;

        public PanelHeader(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/panelheader/PanelHeader.scss");

            _title = Add.Label("", "title");

            OnCreateHeader();

            _closeButton = Add.Button("╳", "closeButton", () =>
            {
                OnClose?.Invoke(this);
            });
        }

        public void SetTitle(string text)
        {
            _title.Text = text;
        }

        public virtual void OnCreateHeader()
        {

        }
    }
}
