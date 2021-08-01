using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class PanelHeader : Panel
    {
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
                if (Parent is TTTPanel tttPanel)
                {
                    tttPanel.IsShowing = false;
                }
                else
                {
                    Parent.Delete(true);
                }
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
