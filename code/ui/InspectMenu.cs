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

        private ConfirmationPanel confirmationPanel { get; set; }

        private ConfirmationHintPanel confirmationHintPanel { get; set; }

        public InspectMenu()
        {
            Instance = this;
            IsShowing = false;

            StyleSheet.Load("/ui/InspectMenu.scss");

            confirmationHintPanel = new ConfirmationHintPanel(this);
            confirmationPanel = new ConfirmationPanel(this);
        }

        public void InspectCorpse(TTTPlayer deadPlayer, bool isIdentified)
        {
            IsShowing = true;

            if (isIdentified)
            {
                confirmationHintPanel.SetClass("hide", true);

                confirmationPanel.SetPlayer(deadPlayer);
                confirmationPanel.SetClass("hide", false);
            }
            else
            {
                confirmationPanel.SetClass("hide", true);
                confirmationHintPanel.SetClass("hide", false);
            }
        }

        private class ConfirmationPanel : Panel
        {
            private Header header { set; get; }

            private Content content { set; get; }

            private Footer footer { set; get; }

            public ConfirmationPanel(Panel parent)
            {
                Parent = parent;

                header = new Header(this);
                content = new Content(this);
                footer = new Footer(this);
            }

            public void SetPlayer(TTTPlayer player)
            {
                header.SetPlayer(player);
                content.SetPlayer(player);
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
                    RoleLabel.Style.Dirty();
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
                    inspectWeapon.ImageWrapper.Image.SetTexture("");
                    inspectWeapon.InspectItemLabel.Text = "Pistol";

                    inspectItems.Add(inspectWeapon);
                }

                public void SetPlayer(TTTPlayer player)
                {
                    PlayerImage.Image.SetTexture($"avatar:{player.GetClientOwner().SteamId}");

                    PlayerImage.Style.BorderColor = player.Role.Color;
                    PlayerImage.Style.Dirty();
                }
            }

            private class ImageWrapper : Panel
            {
                public Image Image { get; set; }

                public ImageWrapper(Panel parent)
                {
                    Parent = parent;

                    Image = Add.Image("", "avatar");
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

                    FooterLabel = Add.Label("0 credits left", "inspect");
                }
            }
        }

        private class ConfirmationHintPanel : Panel
        {
            public Label InspectLabel { get; set; }

            public ConfirmationHintPanel(Panel parent)
            {
                Parent = parent;

                InspectLabel = Add.Label("Press E to confirm", "inspect");
            }
        }
    }
}
