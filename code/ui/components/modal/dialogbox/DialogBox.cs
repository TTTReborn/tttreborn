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

        public DialogBox(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/components/modal/dialogbox/DialogBox.scss");

            AddClass("dialogbox");

            Header.DragHeader.IsLocked = true;

            _agreeButton = Footer.Add.ButtonWithIcon("done", "", "agree", OnClickAgree);
            _declineButton = Footer.Add.ButtonWithIcon("close", "", "decline", OnClickDecline);
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
            return Content.Add.Label(text, "text");
        }

        public Label AddTranslationText(string translationKey, params object[] translationData)
        {
            return Content.Add.TranslationLabel(translationKey, "text", translationData);
        }
    }
}
