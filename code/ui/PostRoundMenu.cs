using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class PostRoundStats
    {
        public string WinningRole { get; private set; }
        public Color WinningColor { get; private set; }

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
        private Header _header;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/PostRoundMenu.scss");
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

            _header.WinnerLabel.Text = $"{_stats.WinningRole.ToUpper()}S WIN!";
            _header.WinnerLabel.Style.FontColor = _stats.WinningColor;
        }

        private class Header : Panel
        {
            public Label WinnerLabel { get; set; }

            public Header(Panel parent)
            {
                Parent = parent;

                WinnerLabel = Add.Label("", "title");
            }
        }
    }
}
