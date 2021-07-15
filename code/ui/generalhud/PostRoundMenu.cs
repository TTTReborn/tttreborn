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
        private readonly Header _header;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/PostRoundMenu.scss");

            _header = new Header(this);
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

            _header.WinnerLabel.Text = $"{_stats.WinningRole.ToUpper()} WIN!";
            _header.Style.BackgroundColor = _stats.WinningColor;
        }

        private class Header : Panel
        {
            public readonly Label WinnerLabel;

            public Header(Panel parent)
            {
                Parent = parent;

                WinnerLabel = Add.Label("", "title");
            }
        }
    }
}
