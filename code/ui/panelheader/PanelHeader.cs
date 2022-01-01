using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public partial class PanelHeader : Panel
    {
        public Action<PanelHeader> OnClose { get; set; }

        private TranslationLabel _title;

        public PanelHeader(Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/panelheader/PanelHeader.scss");

            Reload();
        }

        public void Reload()
        {
            DeleteChildren(true);

            _title = Add.TranslationLabel(new TranslationData(), "title");

            OnCreateHeader();

            Add.ButtonWithIcon(null, "close", "closeButton", () =>
            {
                OnClose?.Invoke(this);
            });
        }

        public void SetTitle(string text)
        {
            _title.Text = text;
        }

        public void SetTranslationTitle(TranslationData translationdata)
        {
            _title.UpdateTranslation(translationdata);
        }

        public virtual void OnCreateHeader()
        {

        }
    }
}
