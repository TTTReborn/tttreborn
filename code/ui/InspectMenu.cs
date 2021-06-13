using System.Collections.Generic;
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
            get => isShowing;
            set
            {
                isShowing = value;

                SetClass("hide", !isShowing);
            }
        }

        private bool isShowing = false;

        private Header header { set; get; }

        private Content content { set; get; }

        private Footer footer { set; get; }

        private ConfirmationPanel confirmationPanel { get; set; }

        public InspectMenu()
        {
            Instance = this;
            IsShowing = false;

            StyleSheet.Load("/ui/InspectMenu.scss");

            header = new Header(this);
            header.AddClass("hide");

            content = new Content(this);
            content.AddClass("hide");

            footer = new Footer(this);
            content.AddClass("hide");

            confirmationPanel = new ConfirmationPanel(this);
        }

        public void InspectCorpse(TTTPlayer deadPlayer, bool isIdentified)
        {
            IsShowing = true;

            if (isIdentified)
            {
                confirmationPanel.SetClass("hide", true);

                header.SetPlayer(deadPlayer);
                header.SetClass("hide", false);

                content.SetPlayer(deadPlayer);
                content.SetClass("hide", false);

                footer.SetClass("hide", false);
            }
            else
            {
                header.SetClass("hide", true);
                content.SetClass("hide", true);
                footer.SetClass("hide", true);

                confirmationPanel.SetClass("hide", false);
            }
        }

        private class Header : Panel
        {
            public Label PlayerLabel { get; set; }

            public Label RoleLabel { get; set; }

            public Header(Panel parent)
            {
                Parent = parent;

                PlayerLabel = Add.Label("", "player");
                RoleLabel = Add.Label("", "role");
            }

            public void SetPlayer(TTTPlayer player)
            {
                PlayerLabel.Text = player.GetClientOwner().Name;

                RoleLabel.Text = player.Role.Name;
                RoleLabel.Style.FontColor = player.Role.Color;
            }
        }

        private class Content : Panel
        {
            public ImageWrapper PlayerImage { get; set; }

            public List<InspectItem> inspectItems { get; set; } = new();

            public Content(Panel parent)
            {
                Parent = parent;

                PlayerImage = new ImageWrapper(this);
                PlayerImage.AddClass("playericon");

                InspectItem inspectWeapon = new InspectItem(this);
                inspectWeapon.ImageWrapper.SetImageSource("");
                inspectWeapon.InspectItemLabel.Text = "Pistol";

                inspectItems.Add(inspectWeapon);
            }

            public void SetPlayer(TTTPlayer player)
            {
                PlayerImage.SetImageSource(""); // TODO fetch player's steam avatar
            }
        }

        private class ImageWrapper : Panel
        {
            public Image Image { get; set; }

            public ImageWrapper(Panel parent)
            {
                Parent = parent;
            }

            public void SetImageSource(string source)
            {
                Image = Add.Image(source);
            }
        }

        private class InspectItem : Panel
        {
            public ImageWrapper ImageWrapper { get; set; }

            public Label InspectItemLabel { get; set; }

            public InspectItem(Panel parent)
            {
                Parent = parent;

                ImageWrapper = new ImageWrapper(this);
                InspectItemLabel = Add.Label("", "inspectItemName");
            }
        }

        private class Footer : Panel
        {
            public Label FooterLabel { get; set; }

            public Footer(Panel parent)
            {
                Parent = parent;

                FooterLabel = Add.Label("x confirmed this body", "inspect");
            }
        }

        private class ConfirmationPanel : Panel
        {
            public Label InspectLabel { get; set; }

            public ConfirmationPanel(Panel parent)
            {
                Parent = parent;

                InspectLabel = Add.Label("Press E to confirm", "inspect");
            }
        }
    }
}
