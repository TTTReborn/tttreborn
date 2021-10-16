using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class DialogBox : Modal
    {
        public Action OnAgree { get; set; }
        public Action OnDecline { get; set; }

        private Button _agreeButton;
        private Button _declineButton;

        public DialogBox() : base()
        {
            StyleSheet.Load("/ui/components/modal/dialogbox/DialogBox.scss");

            AddClass("dialogbox");

            WindowHeader.DragHeaderWrapper.IsLocked = true;

            _agreeButton = WindowFooter.Add.ButtonWithIcon("done", "", "agree", OnClickAgree);
            _declineButton = WindowFooter.Add.ButtonWithIcon("close", "", "decline", OnClickDecline);
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
            return WindowContent.Add.Label(text, "text");
        }
    }
}
