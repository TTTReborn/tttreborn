using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class DialogBox : Modal
    {
        public Action<DialogBox> OnAgree { get; set; }
        public Action<DialogBox> OnDecline { get; set; }

        public Label TitleLabel { get; set; }
        public Panel ContentPanel { get; set; }

        private Panel _headerPanel;
        private Panel _footerPanel;
        private Button _agreeButton;
        private Button _declineButton;

        public DialogBox(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/modal/dialogbox/DialogBox.scss");

            _headerPanel = Add.Panel("header");
            TitleLabel = _headerPanel.Add.Label("", "title");

            ContentPanel = Add.Panel("content");

            _footerPanel = Add.Panel("footer");
            _agreeButton = _footerPanel.Add.ButtonWithIcon("done", "", "agree", () => OnAgree?.Invoke(this));
            _declineButton = _footerPanel.Add.ButtonWithIcon("close", "", "decline", () => OnDecline?.Invoke(this));
        }

        public Label AddText(string text)
        {
            return ContentPanel.Add.Label(text, "text");
        }
    }
}
