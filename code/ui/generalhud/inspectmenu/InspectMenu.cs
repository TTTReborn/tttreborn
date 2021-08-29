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

        public PlayerCorpse PlayerCorpse;

        private readonly ConfirmationPanel _confirmationPanel;

        private readonly ConfirmationHintPanel _confirmationHintPanel;

        public InspectMenu()
        {
            Instance = this;
            Enabled = false;

            StyleSheet.Load("/ui/generalhud/inspectmenu/InspectMenu.scss");

            _confirmationHintPanel = new ConfirmationHintPanel(this);
            _confirmationPanel = new ConfirmationPanel(this);
        }

        public void InspectCorpse(PlayerCorpse playerCorpse)
        {
            if (playerCorpse == null)
            {
                return;
            }

            Enabled = true;
            PlayerCorpse = playerCorpse;

            if (playerCorpse.IsIdentified)
            {
                _confirmationHintPanel.Enabled = false;

                _confirmationPanel.SetPlayer(playerCorpse.Player);
                _confirmationPanel.SetConfirmationData(playerCorpse.GetConfirmationData());
                _confirmationPanel.SetKillerWeapon(playerCorpse.KillerWeapon);
                _confirmationPanel.SetPerks(playerCorpse.Perks);
                _confirmationPanel.Enabled = true;

                _confirmationPanel.Style.BorderColor = playerCorpse.Player.Role.Color;
                _confirmationPanel.Style.Dirty();
            }
            else
            {
                _confirmationPanel.Enabled = false;
                _confirmationHintPanel.Enabled = true;
            }
        }

        private class ConfirmationPanel : Panel
        {
            private readonly Header _header;

            private readonly Content _content;

            private readonly Footer _footer;

            public ConfirmationPanel(Sandbox.UI.Panel parent)
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

            public void SetPerks(string[] perks)
            {
                _content.SetPerks(perks);
            }

            private class Header : Panel
            {
                private readonly Label _playerLabel;

                private readonly Label _roleLabel;

                public Header(Sandbox.UI.Panel parent)
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
                private List<InspectItem> _perksList = new();
                private ConfirmationData _confirmationData;

                private readonly ImageWrapper _playerImage;

                public Content(Sandbox.UI.Panel parent)
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
                    _timeSinceDeath.ImageWrapper.Image.SetTexture($"/ui/inspectmenu/time.png");
                    _timeSinceDeath.InspectItemLabel.Text = "";

                    _headshot?.Delete(true);

                    if (confirmationData.Headshot)
                    {
                        _headshot = new InspectItem(this);
                        _headshot.ImageWrapper.Image.SetTexture($"/ui/inspectmenu/headshot.png");
                        _headshot.InspectItemLabel.Text = "By a headshot";
                    }

                    _distance?.Delete(true);
                    _suicide?.Delete(true);

                    if (!confirmationData.Suicide)
                    {
                        _distance = new InspectItem(this);
                        _distance.ImageWrapper.Image.SetTexture($"/ui/inspectmenu/distance.png");
                        _distance.InspectItemLabel.Text = $"From {confirmationData.Distance:n0}m away";
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
                        _killerWeapon.InspectItemLabel.Text = $"With a {killerWeapon}";
                    }
                }

                public void SetPerks(string[] perks)
                {
                    foreach (InspectItem loopItem in _perksList)
                    {
                        loopItem.Delete(true);
                    }

                    _perksList.Clear();

                    if (perks == null)
                    {
                        return;
                    }

                    foreach (string perkName in perks)
                    {
                        InspectItem inspectItem = new InspectItem(this);
                        inspectItem.ImageWrapper.Image.SetTexture($"/ui/weapons/{perkName}.png");
                        inspectItem.InspectItemLabel.Text = perkName;

                        _perksList.Add(inspectItem);
                    }
                }

                public override void Tick()
                {
                    base.Tick();

                    if (_timeSinceDeath != null && _timeSinceDeath.IsVisible)
                    {
                        string[] timeSplits = TimeSpan.FromSeconds(Math.Round(Time.Now - _confirmationData.Time)).ToString().Split(':');

                        _timeSinceDeath.InspectItemLabel.Text = $"Died {timeSplits[1]}:{timeSplits[2]} ago";
                    }
                }
            }

            private class ImageWrapper : Panel
            {
                public readonly Image Image;

                public ImageWrapper(Sandbox.UI.Panel parent)
                {
                    Parent = parent;

                    Image = Add.Image("", "avatar");
                }
            }

            private class InspectItem : Panel
            {
                public readonly ImageWrapper ImageWrapper;

                public readonly Label InspectItemLabel;

                public InspectItem(Sandbox.UI.Panel parent)
                {
                    Parent = parent;

                    ImageWrapper = new ImageWrapper(this);
                    InspectItemLabel = Add.Label("", "inspectItemName");
                }
            }

            private class Footer : Panel
            {
                private readonly Label _footerLabel;

                public Footer(Sandbox.UI.Panel parent)
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

            public ConfirmationHintPanel(Sandbox.UI.Panel parent)
            {
                Parent = parent;

                _inspectLabel = Add.Label("Press E to confirm", "inspect");
            }
        }
    }
}
