using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
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

                SetClass("hide", !_isShowing);
            }
        }
        private bool _isShowing = false;

        private readonly ConfirmationPanel _confirmationPanel;

        private readonly ConfirmationHintPanel _confirmationHintPanel;

        public InspectMenu()
        {
            Instance = this;
            IsShowing = false;

            StyleSheet.Load("/ui/InspectMenu.scss");

            _confirmationHintPanel = new ConfirmationHintPanel(this);
            _confirmationPanel = new ConfirmationPanel(this);
        }

        public void InspectCorpse(TTTPlayer deadPlayer, bool isIdentified)
        {
            IsShowing = true;

            if (isIdentified)
            {
                _confirmationHintPanel.SetClass("hide", true);

                _confirmationPanel.SetPlayer(deadPlayer);
                _confirmationPanel.SetClass("hide", false);
            }
            else
            {
                _confirmationPanel.SetClass("hide", true);
                _confirmationHintPanel.SetClass("hide", false);
            }
        }

        private class ConfirmationPanel : Panel
        {
            private readonly Header _header;

            private readonly Content _content;

            private readonly Footer _footer;

            public ConfirmationPanel(Panel parent)
            {
                Parent = parent;

                _header = new Header(this);
                _content = new Content(this);
                _footer = new Footer(this);
            }

            public void SetPlayer(TTTPlayer player)
            {
                _header.SetPlayer(player);
                _content.SetPlayer(player);
                _footer.SetPlayer(player);
            }

            private class Header : Panel
            {
                private readonly Label _playerLabel;

                private readonly Label _roleLabel;

                public Header(Panel parent)
                {
                    Parent = parent;

                    _playerLabel = Add.Label("", "player");
                    _roleLabel = Add.Label("", "role");
                }

                public void SetPlayer(TTTPlayer player)
                {
                    _playerLabel.Text = player.GetClientOwner().Name;

                    _roleLabel.Text = player.Role.Name;
                    _roleLabel.Style.FontColor = player.Role.Color;
                    _roleLabel.Style.Dirty();
                }
            }

            private class Content : Panel
            {
                private readonly ImageWrapper _playerImage;

                private readonly List<InspectItem> _inspectItems = new();

                public Content(Panel parent)
                {
                    Parent = parent;

                    _playerImage = new ImageWrapper(this);
                    _playerImage.AddClass("playericon");

                    InspectItem inspectWeapon = new InspectItem(this);
                    inspectWeapon.ImageWrapper.Image.SetTexture("");
                    inspectWeapon.InspectItemLabel.Text = "Pistol";

                    _inspectItems.Add(inspectWeapon);
                }

                public void SetPlayer(TTTPlayer player)
                {
                    _playerImage.Image.SetTexture($"avatar:{player.GetClientOwner().SteamId}");

                    _playerImage.Style.BorderColor = player.Role.Color;
                    _playerImage.Style.Dirty();
                }
            }

            private class ImageWrapper : Panel
            {
                public readonly Image Image;

                public ImageWrapper(Panel parent)
                {
                    Parent = parent;

                    Image = Add.Image("", "avatar");
                }
            }

            private class InspectItem : Panel
            {
                public readonly ImageWrapper ImageWrapper;

                public readonly Label InspectItemLabel;

                public InspectItem(Panel parent)
                {
                    Parent = parent;

                    ImageWrapper = new ImageWrapper(this);
                    InspectItemLabel = Add.Label("", "inspectItemName");
                }
            }

            private class Footer : Panel
            {
                private readonly Label _footerLabel;

                public Footer(Panel parent)
                {
                    Parent = parent;

                    _footerLabel = Add.Label("$ 0 credits left", "inspect");
                }

                public void SetPlayer(TTTPlayer player)
                {
                    if (player.CorpseConfirmer != Local.Pawn as TTTPlayer)
                    {
                        _footerLabel.SetClass("hide", true);

                        return;
                    }

                    _footerLabel.SetClass("hide", false);

                    _footerLabel.Text = $"$ {player.CorpseCredits} credits found";
                }
            }
        }

        private class ConfirmationHintPanel : Panel
        {
            private Label _inspectLabel;

            public ConfirmationHintPanel(Panel parent)
            {
                Parent = parent;

                _inspectLabel = Add.Label("Press E to confirm", "inspect");
            }
        }
    }
}
