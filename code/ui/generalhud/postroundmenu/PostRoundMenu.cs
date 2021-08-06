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

    public class PostRoundMenu : TTTPanel
    {
        public static PostRoundMenu Instance;

        private bool _isShowing = false;

        private PostRoundStats _stats;
        private readonly Label _headerLabel;
        private readonly Label _contentLabel;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/postroundmenu/PostRoundMenu.scss");

            _headerLabel = Add.Label("", "headerLabel");

            _contentLabel = Add.Label("Thanks for playing TTT Reborn, more updates and stats to come!", "contentLabel");

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

            _headerLabel.Text = $"{_stats.WinningRole.ToUpper()} WIN!";
            _headerLabel.Style.BackgroundColor = _stats.WinningColor;
        }
    }
}
