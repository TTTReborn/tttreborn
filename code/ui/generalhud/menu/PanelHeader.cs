using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class PanelHeader : Panel
    {
        private Label _title;

        private Button _closeButton;

        public PanelHeader() : base()
        {
            StyleSheet.Load("/ui/generalhud/menu/PanelHeader.scss");

            _title = Add.Label("", "title");

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
    }
}
