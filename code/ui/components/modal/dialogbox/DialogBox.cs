using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class DialogBox : Modal
    {
        public Action OnAgree { get; set; }
        public Action OnDecline { get; set; }

        public readonly Drag HeaderPanel;
        public readonly Label TitleLabel;
        public readonly Panel ContentPanel;

        private Panel _footerPanel;
        private Button _agreeButton;
        private Button _declineButton;

        public DialogBox() : base()
        {
            StyleSheet.Load("/ui/components/modal/dialogbox/DialogBox.scss");

            AddClass("dialogbox");

            HeaderPanel = new Drag(this);
            HeaderPanel.AddClass("header");
            HeaderPanel.DragBasePanel = this;
            HeaderPanel.IsLocked = true;

            TitleLabel = HeaderPanel.Add.Label("", "title");

            ContentPanel = Add.Panel("content");

            _footerPanel = Add.Panel("footer");
            _agreeButton = _footerPanel.Add.ButtonWithIcon("done", "", "agree", OnClickAgree);
            _declineButton = _footerPanel.Add.ButtonWithIcon("close", "", "decline", OnClickDecline);
        }

        public virtual void OnClickAgree()
        {
            OnAgree?.Invoke();
        }

        public virtual void OnClickDecline()
        {
            OnDecline?.Invoke();
        }

        public Label AddText(string text)
        {
            return ContentPanel.Add.Label(text, "text");
        }
    }
}
