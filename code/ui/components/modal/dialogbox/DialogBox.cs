using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public partial class DialogBox : Modal
    {
        public Action OnAgree { get; set; }
        public Action OnDecline { get; set; }

        public DialogBox() : base()
        {
            StyleSheet.Load("/ui/components/modal/dialogbox/DialogBox.scss");

            AddClass("dialogbox");

            Header.DragHeader.IsLocked = true;

            _ = Footer.Add.ButtonWithIcon("", "done", "agree", OnClickAgree);
            _ = Footer.Add.ButtonWithIcon("", "close", "decline", OnClickDecline);
        }

        public virtual void OnClickAgree()
        {
            OnAgree?.Invoke();
        }

        public virtual void OnClickDecline()
        {
            OnDecline?.Invoke();
        }

        public Label AddText(string text) => Content.Add.Label(text, "text");

        public Label AddTranslation(TranslationData translationData) => Content.Add.TranslationLabel(translationData, "text");
    }
}
