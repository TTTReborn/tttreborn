using Sandbox.UI;
using Sandbox.UI.Construct;
using TTTReborn.Gamemode;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class InspectMenu : Panel
    {
        public static InspectMenu Instance;

        private Label InspectLabel { set; get; }

        public InspectMenu()
        {
            Instance = this;
            StyleSheet.Load("/ui/InspectMenu.scss");

            InspectLabel = Add.Label("Test");

            Show(false);
        }

        public void InspectCorpse(TTTPlayer deadPlayer)
        {
            Show(true);
            InspectLabel.Text = deadPlayer.Role.ToString();
        }

        public void Show(bool isShowing)
        {
            if (isShowing)
            {
                RemoveClass("hide");
                AddClass("show");
            }
            else
            {
                RemoveClass("show");
                AddClass("hide");
            }
        }
    }

}
