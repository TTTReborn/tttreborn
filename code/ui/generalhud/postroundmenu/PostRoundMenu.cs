using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

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

        private readonly Panel _backgroundBannerPanel;
        private readonly Panel _containerPanel;

        private readonly TranslationLabel _headerLabel;
        private readonly TranslationLabel _contentLabel;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/postroundmenu/PostRoundMenu.scss");

            AddClass("text-shadow");

            _backgroundBannerPanel = new(this);
            _backgroundBannerPanel.AddClass("background-color-secondary");
            _backgroundBannerPanel.AddClass("background-banner-panel");
            _backgroundBannerPanel.AddClass("opacity-medium");

            _containerPanel = new(_backgroundBannerPanel);
            _containerPanel.AddClass("container-panel");

            _headerLabel = _containerPanel.Add.TranslationLabel(new TranslationData());
            _headerLabel.AddClass("header-label");

            _contentLabel = _containerPanel.Add.TranslationLabel(new TranslationData());
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

            _contentLabel.UpdateTranslation(new TranslationData("POST_ROUND_TEXT"));

            _headerLabel.UpdateTranslation(new TranslationData($"POST_ROUND_WIN_{_stats.WinningRole.ToUpper()}"));
            _headerLabel.Style.FontColor = _stats.WinningColor;
        }
    }
}
