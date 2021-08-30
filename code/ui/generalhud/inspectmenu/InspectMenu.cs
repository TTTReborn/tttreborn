using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class InspectMenu : TTTPanel
    {
        public static InspectMenu Instance;

        private PlayerCorpse _playerCorpse;

        private readonly Header _header;
        private readonly Content _content;

        public InspectMenu()
        {
            Instance = this;
            StyleSheet.Load("/ui/generalhud/inspectmenu/InspectMenu.scss");

            _header = new Header(this);
            _content = new Content(this);

            IsShowing = false;
        }

        public void InspectCorpse(PlayerCorpse playerCorpse)
        {
            if (playerCorpse?.Player == null)
            {
                return;
            }

            _playerCorpse = playerCorpse;
            _header.SetPlayerData(_playerCorpse.Player);
            _content.SetConfirmationData(_playerCorpse.GetConfirmationData(), _playerCorpse.KillerWeapon, _playerCorpse.Perks);

            IsShowing = true;
        }

        public void SetPlayerData(PlayerCorpse playerCorpse)
        {
            _playerCorpse = playerCorpse;
            _header.SetPlayerData(_playerCorpse.Player);
            _content.SetConfirmationData(_playerCorpse.GetConfirmationData(), _playerCorpse.KillerWeapon, _playerCorpse.Perks);
        }

        private class Header : TTTPanel
        {
            private readonly Label _playerLabel;
            private readonly Label _roleLabel;

            public Header(TTTPanel parent)
            {
                Parent = parent;

                _playerLabel = Add.Label(String.Empty, "player");
                _roleLabel = Add.Label(String.Empty, "role");

                Button button = Add.Button("X", () =>
                {
                    parent.IsShowing = false;
                });
                button.AddClass("closeButton");
            }

            public void SetPlayerData(TTTPlayer player)
            {
                _playerLabel.Text = player?.GetClientOwner().Name;

                _roleLabel.Text = player?.Role.Name;
                _roleLabel.Style.FontColor = player?.Role.Color;
                _roleLabel.Style.Dirty();
            }
        }

        private class Content : TTTPanel
        {
            private ConfirmationData _confirmationData;

            // Unique case: Every corpse has a time since death icon, we need a reference to update the clock each tick.
            private readonly InspectIcon _timeSinceDeathIcon;
            private readonly IconsWrapper _iconsWrapper;

            public Content(Panel parent)
            {
                Parent = parent;
                _iconsWrapper = new IconsWrapper(this, new DescriptionWrapper(this));
                _timeSinceDeathIcon = _iconsWrapper.Add(new InspectIconData{Description = String.Empty, ImagePath = $"/ui/inspectmenu/time.png", IsUnique = false});
                _iconsWrapper.SelectIcon(_timeSinceDeathIcon);
            }

            public override void Tick()
            {
                base.Tick();

                string[] timeSplits = TimeSpan.FromSeconds(Math.Round(Time.Now - _confirmationData.Time)).ToString().Split(':');
                _timeSinceDeathIcon.IconData.Description = $"Died {timeSplits[1]}:{timeSplits[2]} ago";
            }

            public void SetConfirmationData(ConfirmationData confirmationData, string killerWeapon, string[] perks)
            {
                _confirmationData = confirmationData;

                _iconsWrapper.ClearUniqueIcons();

                if (confirmationData.Headshot)
                {
                    _iconsWrapper.Add(new InspectIconData{Description = $"This body is missing a head.", ImagePath = "/ui/inspectmenu/headshot.png", IsUnique = true});
                }

                if (!confirmationData.Suicide)
                {
                    _iconsWrapper.Add(new InspectIconData{Description = $"From {confirmationData.Distance:n0}m away", ImagePath = "/ui/inspectmenu/distance.png", IsUnique = true});
                }
                else
                {
                    _iconsWrapper.Add(new InspectIconData{Description = $"Committed suicide.", ImagePath = String.Empty, IsUnique = true});
                }

                if (!string.IsNullOrEmpty(killerWeapon))
                {
                    _iconsWrapper.Add(new InspectIconData{Description = $"With a {killerWeapon}", ImagePath = $"/ui/weapons/{killerWeapon}.png", IsUnique = true});
                }

                if (perks != null)
                {
                    foreach (string perkName in perks)
                    {
                        _iconsWrapper.Add(new InspectIconData{Description = perkName, ImagePath = $"/ui/weapons/{perkName}.png", IsUnique = true});
                    }
                }
            }
        }

        private class DescriptionWrapper : TTTPanel
        {
            public readonly Label DescriptionLabel;

            public DescriptionWrapper(Panel parent)
            {
                Parent = parent;
                DescriptionLabel = Add.Label(String.Empty, "descriptionLabel");
            }
        }

        private class IconsWrapper : TTTPanel
        {
            private readonly DescriptionWrapper _descriptionWrapper;
            private InspectIcon _selectedIcon;
            private readonly List<InspectIcon> _uniqueIcons = new();

            public IconsWrapper(Panel parent, DescriptionWrapper descriptionWrapper)
            {
                Parent = parent;
                _descriptionWrapper = descriptionWrapper;
            }

            public new InspectIcon Add(InspectIconData iconData)
            {
                InspectIcon icon = new InspectIcon(this, iconData);
                icon.AddEventListener("onclick", () =>
                {
                    SelectIcon(icon);
                });

                if (iconData.IsUnique)
                {
                    _uniqueIcons.Add(icon);
                }

                return icon;
            }

            public override void Tick()
            {
                _descriptionWrapper.DescriptionLabel.Text = _selectedIcon?.IconData.Description;
            }

            public void SelectIcon(InspectIcon icon)
            {
                _selectedIcon = icon;
                foreach (Panel iconPanel in Children)
                {
                    iconPanel.SetClass("selected", iconPanel == icon);
                }
            }

            public void ClearUniqueIcons()
            {
                _uniqueIcons.ForEach((i) => i.Delete(true));
                _uniqueIcons.Clear();
            }
        }
    }
}
