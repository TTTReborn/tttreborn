using System;
using System.Collections.Generic;

using Sandbox;
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

            StyleSheet.Load("/ui/alivehud/inspectmenu/InspectMenu.scss");

            _confirmationHintPanel = new ConfirmationHintPanel(this);
            _confirmationPanel = new ConfirmationPanel(this);
        }

        public void InspectCorpse(TTTPlayer deadPlayer, bool isIdentified, ConfirmationData confirmationData, string killerWeapon)
        {
            IsShowing = true;

            if (isIdentified)
            {
                _confirmationHintPanel.SetClass("hide", true);

                _confirmationPanel.SetPlayer(deadPlayer);
                _confirmationPanel.SetConfirmationData(confirmationData);
                _confirmationPanel.SetKillerWeapon(killerWeapon);
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

            public void SetConfirmationData(ConfirmationData confirmationData)
            {
                _content.SetConfirmationData(confirmationData);
            }

            public void SetKillerWeapon(string killerWeapon)
            {
                _content.SetKillerWeapon(killerWeapon);
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
                private InspectItem _timeSinceDeath;
                private InspectItem _killerWeapon;
                private InspectItem _headshot;
                private InspectItem _distance;
                private InspectItem _suicide;
                private ConfirmationData _confirmationData;

                private readonly ImageWrapper _playerImage;

                public Content(Panel parent)
                {
                    Parent = parent;

                    _playerImage = new ImageWrapper(this);
                    _playerImage.AddClass("playericon");
                }

                public void SetPlayer(TTTPlayer player)
                {
                    _playerImage.Image.SetTexture($"avatar:{player.GetClientOwner().SteamId}");

                    _playerImage.Style.BorderColor = player.Role.Color;
                    _playerImage.Style.Dirty();
                }

                public void SetConfirmationData(ConfirmationData confirmationData)
                {
                    _confirmationData = confirmationData;

                    _timeSinceDeath?.Delete(true);

                    _timeSinceDeath = new InspectItem(this);
                    _timeSinceDeath.ImageWrapper.Image.SetTexture("");
                    _timeSinceDeath.InspectItemLabel.Text = "";

                    _headshot?.Delete(true);

                    if (confirmationData.Headshot)
                    {
                        _headshot = new InspectItem(this);
                        _headshot.ImageWrapper.Image.SetTexture("");
                        _headshot.InspectItemLabel.Text = "Headshot";
                    }

                    _distance?.Delete(true);
                    _suicide?.Delete(true);

                    if (!confirmationData.Suicide)
                    {
                        _distance = new InspectItem(this);
                        _distance.ImageWrapper.Image.SetTexture("");
                        _distance.InspectItemLabel.Text = $"Distance: {confirmationData.Distance:n0}";
                    }
                    else
                    {
                        _suicide = new InspectItem(this);
                        _suicide.ImageWrapper.Image.SetTexture("");
                        _suicide.InspectItemLabel.Text = $"Committed suicide";
                    }
                }

                public void SetKillerWeapon(string killerWeapon)
                {
                    _killerWeapon?.Delete(true);

                    if (killerWeapon != null)
                    {
                        _killerWeapon = new InspectItem(this);
                        _killerWeapon.ImageWrapper.Image.SetTexture($"/ui/weapons/{killerWeapon}.png");
                        _killerWeapon.InspectItemLabel.Text = killerWeapon;
                    }
                }

                public override void Tick()
                {
                    if (_timeSinceDeath != null && _timeSinceDeath.IsVisible)
                    {
                        string[] timeSplits = TimeSpan.FromSeconds(Math.Round(Time.Now - _confirmationData.Time)).ToString().Split(':');

                        _timeSinceDeath.InspectItemLabel.Text = $"Died {timeSplits[1]}:{timeSplits[2]} ago.";
                    }
                }
            }

            private class ImageWrapper : Panel
            {
                public readonly Image Image;

                public ImageWrapper(Panel parent)
                {
                    Parent = parent;

                    Image = Add.Image("", "image");
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
