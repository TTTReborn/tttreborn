using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class InspectMenu : Panel
    {
        public static InspectMenu Instance;
        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                _isShowing = value;
                if (_isShowing)
                {
                    RemoveClass("hide");
                }
                else
                {
                    AddClass("hide");
                }
            }
        }
        private bool _isShowing = false;

        private Label InspectLabel { set; get; }

        public InspectMenu()
        {
            Instance = this;
            StyleSheet.Load("/ui/InspectMenu.scss");

            InspectLabel = Add.Label("");

            IsShowing = false;
        }

        public void InspectCorpse(TTTPlayer deadPlayer, bool isIdentified)
        {
            IsShowing = true;
            // TODO: Setup proper hud, for now everything is just being thrown into "InspectLabel"
            if (isIdentified)
            {
                InspectLabel.Text = $"{deadPlayer.GetClientOwner()?.Name}\n" +
                                    $"{deadPlayer.Role.ToString()}";
            }
            else
            {
                InspectLabel.Text = "Press E to identify this body.";
            }
        }
    }

}
