using Sandbox.UI;
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

    public class PostRoundMenu : Panel
    {
        public static PostRoundMenu Instance;

        private PostRoundStats _stats;

        private readonly Panel _backgroundPanel;
        private readonly Panel _containerPanel;

        private readonly TranslationLabel _headerLabel;
        private readonly TranslationLabel _contentLabel;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/postroundmenu/PostRoundMenu.scss");

            AddClass("text-shadow");

            _backgroundPanel = new(this);
            _backgroundPanel.AddClass("background-color-secondary");
            _backgroundPanel.AddClass("opacity-medium");
            _backgroundPanel.AddClass("centered");
            _backgroundPanel.AddClass("fullscreen");

            _containerPanel = new(this);
            _containerPanel.AddClass("container-panel");

            _headerLabel = _containerPanel.Add.TranslationLabel("");
            _headerLabel.AddClass("header-label");

            _contentLabel = _containerPanel.Add.TranslationLabel("");
            _contentLabel.AddClass("content-label");
        }

        public void OpenAndSetPostRoundMenu(PostRoundStats stats)
        {
            _stats = stats;

            OpenPostRoundMenu();
        }

        public void ClosePostRoundMenu()
        {
            SetClass("fade-in", false);
            _containerPanel.SetClass("pop-in", false);
        }

        public void OpenPostRoundMenu()
        {
            SetClass("fade-in", true);
            _containerPanel.SetClass("pop-in", true);

            _contentLabel.SetTranslation("POST_ROUND_TEXT");

            _headerLabel.SetTranslation($"POST_ROUND_WIN_{_stats.WinningRole.ToUpper()}");
            _headerLabel.Style.FontColor = _stats.WinningColor;
        }
    }
}
