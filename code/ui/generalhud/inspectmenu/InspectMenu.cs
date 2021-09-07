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

        private PlayerCorpse _playerCorpse;

        private readonly Header _header;
        private readonly Content _content;

        public new bool Enabled
        {
            get => base._isEnabled;
            set
            {
                base._isEnabled = value;

                SetClass("opacity-90", base._isEnabled);
                SetClass("disabled", !base._isEnabled);
            }
        }

        public InspectMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/inspectmenu/InspectMenu.scss");

            AddClass("background-color-secondary");
            AddClass("text-shadow");

            _header = new Header(this);
            _content = new Content(this);

            Enabled = false;
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

            Enabled = true;
        }

        public void SetPlayerData(PlayerCorpse playerCorpse)
        {
            _playerCorpse = playerCorpse;
            _header.SetPlayerData(_playerCorpse.Player);
            _content.SetConfirmationData(playerCorpse.GetConfirmationData(), _playerCorpse.KillerWeapon, _playerCorpse.Perks);
        }

        private class Header : Panel
        {
            private readonly Label _playerLabel;
            private readonly Label _roleLabel;

            public Header(Panel parent)
            {
                Parent = parent;

                _playerLabel = Add.Label(String.Empty, "player");
                _roleLabel = Add.Label(String.Empty, "role");
            }

            public void SetPlayerData(TTTPlayer player)
            {
                _playerLabel.Text = player?.GetClientOwner().Name;

                _roleLabel.Text = player?.Role.Name;
                _roleLabel.Style.FontColor = player?.Role.Color;
                _roleLabel.Style.Dirty();
            }
        }

        private class Content : Panel
        {
            private ConfirmationData _confirmationData;

            // Unique case: Every corpse has a time since death icon, we need a reference to update the clock each tick.
            private readonly InspectIcon _timeSinceDeathIcon;
            private readonly IconsWrapper _iconsWrapper;

            public Content(Panel parent)
            {
                Parent = parent;
                _iconsWrapper = new IconsWrapper(this, new DescriptionWrapper(this));

                _timeSinceDeathIcon = _iconsWrapper.Add(new InspectIconData(false, $"/ui/inspectmenu/time.png", String.Empty));
                _iconsWrapper.SelectIcon(_timeSinceDeathIcon);
            }

            public override void Tick()
            {
                base.Tick();

                string[] timeSplits = TimeSpan.FromSeconds(Math.Round(Time.Now - _confirmationData.Time)).ToString().Split(':');
                _timeSinceDeathIcon.IconData.Description = $"They died roughly {timeSplits[1]}:{timeSplits[2]} ago.";
            }

            public void SetConfirmationData(ConfirmationData confirmationData, string killerWeapon, string[] perks)
            {
                _confirmationData = confirmationData;

                _iconsWrapper.ClearUniqueIcons();
                _iconsWrapper.SelectIcon(_timeSinceDeathIcon);

                if (confirmationData.Headshot)
                {
                    _iconsWrapper.Add(new InspectIconData(true, "/ui/inspectmenu/headshot.png",
                        "The fatal wound was a headshot. No time to scream."));
                }

                if (confirmationData.Suicide)
                {
                    _iconsWrapper.Add(new InspectIconData(true, String.Empty,
                        "You cannot find a specific cause of this Terry's death."));
                }
                else
                {
                    _iconsWrapper.Add(new InspectIconData(true, "/ui/inspectmenu/distance.png",
                        $"They were killed from approximately {confirmationData.Distance:n0}m away."));
                }

                if (!string.IsNullOrEmpty(killerWeapon))
                {
                    _iconsWrapper.Add(new InspectIconData(true, $"/ui/weapons/{killerWeapon}.png",
                        $"It appears a {killerWeapon} was used to kill them."));
                }

                if (perks != null)
                {
                    foreach (string perkName in perks)
                    {
                        _iconsWrapper.Add(new InspectIconData(true, $"/ui/weapons/{perkName}.png",
                            $"They were carrying a {perkName}."));
                    }
                }
            }
        }

        private class DescriptionWrapper : Panel
        {
            public readonly Label DescriptionLabel;

            public DescriptionWrapper(Panel parent)
            {
                Parent = parent;
                DescriptionLabel = Add.Label(String.Empty);
            }
        }

        private class IconsWrapper : Panel
        {
            private InspectIcon _selectedIcon;
            private readonly DescriptionWrapper _descriptionWrapper;

            // Keep a reference to any icons that can uniquely exist on a corpse. As we will need to
            // potentially delete these icons if we inspect a new corpse that doesn't have these icons.
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
            }

            public void ClearUniqueIcons()
            {
                _uniqueIcons.ForEach((i) => i.Delete(true));
                _uniqueIcons.Clear();
            }
        }
    }
}
