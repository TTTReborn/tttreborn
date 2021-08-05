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

    public class PostRoundMenu : Panel
    {
        public static PostRoundMenu Instance;

        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                _isShowing = value;

                SetClass("hide", !_isShowing);
            }
        }
        private bool _isShowing = false;

        private PostRoundStats _stats;
        private readonly Label _headerLabel;
        private readonly Label _contentLabel;
        private TLanguage lang;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/postroundmenu/PostRoundMenu.scss");

            lang = ILanguage.GetActiveLanguage();

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
            lang = ILanguage.GetActiveLanguage();

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
