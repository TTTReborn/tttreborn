using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class PostRoundMenu : Panel
    {
        public static PostRoundMenu Instance;
        // TODO: Create a "PostRoundStats" object that contains all Post Round Data, pass that in instead...
        public string Winner;
        public bool IsShowing
        {
            get => isShowing;
            set
            {
                isShowing = value;

                SetClass("hide", !isShowing);
            }
        }
        private bool isShowing = false;

        private Header header;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/PostRoundMenu.scss");
            header = new Header(this);

            IsShowing = false;
        }

        public void OpenAndSetPostRoundMenu(string winner)
        {
            Winner = winner;

            OpenPostRoundMenu();
        }

        public void OpenPostRoundMenu()
        {
            IsShowing = true;

            header.WinnerLabel.Text = $"{Winner?.ToUpper()}S WIN!";
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
