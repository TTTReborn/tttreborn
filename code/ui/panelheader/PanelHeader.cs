using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class PanelHeader : Panel
    {
        public Action<PanelHeader> OnClose { get; set; }

        private TranslationLabel _title;

        private Button _closeButton;

        public PanelHeader(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/panelheader/PanelHeader.scss");

            Reload();
        }

        public void Reload()
        {
            DeleteChildren(true);

            _title = Add.TryTranslationLabel("", "title");

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

        public void SetTranslationTitle(string translationKey, params object[] translationData)
        {
            _title.SetTranslation(translationKey, translationData);
        }

        public virtual void OnCreateHeader()
        {

        }
    }
}
