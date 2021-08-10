using Sandbox.UI.Construct;

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

        private PostRoundStats _stats;
        private readonly TranslationLabel _headerLabel;
        private readonly TranslationLabel _contentLabel;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/postroundmenu/PostRoundMenu.scss");

            _headerLabel = Add.TranslationLabel("", "headerLabel");

            _contentLabel = Add.TranslationLabel("", "contentLabel");

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

            _contentLabel.SetTranslation("POST_ROUND_TEXT");

            _headerLabel.SetTranslation($"POST_ROUND_WIN_{_stats.WinningRole.ToUpper()}");
            _headerLabel.Style.BackgroundColor = _stats.WinningColor;
        }
    }
}
