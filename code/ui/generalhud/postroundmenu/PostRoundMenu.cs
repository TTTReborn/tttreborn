using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Language;

namespace TTTReborn.UI
{
    public class PostRoundStats
    {
        public readonly string WinningRole;
        public Color WinningColor;

        public PostRoundStats(string winningRole, Color winningColor)
        {
            WinningRole = winningRole;
            WinningColor = winningColor;
        }
    }

    public class PostRoundMenu : TTTPanel
    {
        public static PostRoundMenu Instance;

        private bool _isShowing = false;

        private PostRoundStats _stats;
        private readonly Label _headerLabel;
        private readonly Label _contentLabel;
        private TLanguage lang;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/postroundmenu/PostRoundMenu.scss");

            lang = TTTLanguage.GetActiveLanguage();

            _headerLabel = Add.Label("", "headerLabel");

            _contentLabel = Add.Label("", "contentLabel");

            IsShowing = false;
        }

        public void OpenAndSetPostRoundMenu(PostRoundStats stats)
        {
            _stats = stats;

            OpenPostRoundMenu();
        }

        public void OpenPostRoundMenu()
        {
            IsShowing = true;
            lang = TTTLanguage.GetActiveLanguage();

            _contentLabel.Text = lang.GetTranslation("POST_ROUND_TEXT");

            switch (_stats.WinningRole)
            {
                case "Innocents":
                    _headerLabel.Text = lang.GetTranslation("POST_ROUND_WIN_INNOCENT");
                    break;

                case "Traitors":
                    _headerLabel.Text = lang.GetTranslation("POST_ROUND_WIN_TRAITORS");
                    break;
            }

            _headerLabel.Style.BackgroundColor = _stats.WinningColor;
        }
    }
}
