using System;
using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    using System.Linq;

    using Sandbox;

    public class InspectMenu : TTTPanel
    {
        public static InspectMenu Instance;

        public PlayerCorpse PlayerCorpse;

        private readonly ConfirmationPanel _confirmationPanel;
        private readonly ConfirmCorpsePanel _confirmCorpsePanel;

        public InspectMenu()
        {
            Instance = this;
            StyleSheet.Load("/ui/generalhud/inspectmenu/InspectMenu.scss");

            _confirmationPanel = new ConfirmationPanel(this);
            _confirmCorpsePanel = new ConfirmCorpsePanel(this);

            IsShowing = false;
        }

        public void InspectCorpse(PlayerCorpse playerCorpse)
        {
            if (playerCorpse == null)
            {
                return;
            }

            IsShowing = true;
            PlayerCorpse = playerCorpse;

            if (playerCorpse.IsIdentified)
            {
                _confirmCorpsePanel.IsShowing = false;

                _confirmationPanel.SetPlayerData(playerCorpse.Player);
                _confirmationPanel.SetConfirmationData(playerCorpse.GetConfirmationData(), playerCorpse.KillerWeapon, playerCorpse.Perks);
                _confirmationPanel.IsShowing = true;
            }
            else
            {
                _confirmationPanel.IsShowing = false;
                _confirmCorpsePanel.IsShowing = true;
            }
        }

        private class ConfirmationPanel : TTTPanel
        {
            private readonly Header _header;
            private readonly Content _content;

            public ConfirmationPanel(Panel parent)
            {
                Parent = parent;

                _header = new Header(this);
                _content = new Content(this);
            }

            public void SetPlayerData(TTTPlayer player)
            {
                _header.SetPlayerData(player);
            }

            public void SetConfirmationData(ConfirmationData confirmationData, string killerWeapon, string[] perks)
            {
                _content.SetConfirmationData(confirmationData, killerWeapon, perks);
            }

            private class Header : TTTPanel
            {
                private readonly Label _playerLabel;

                private readonly Label _roleLabel;

                public Header(Panel parent)
                {
                    Parent = parent;

                    _playerLabel = Add.Label("", "player");
                    _roleLabel = Add.Label("", "role");
                }

                public void SetPlayerData(TTTPlayer player)
                {
                    _playerLabel.Text = player.GetClientOwner().Name;

                    _roleLabel.Text = player.Role.Name;
                    _roleLabel.Style.FontColor = player.Role.Color;
                    _roleLabel.Style.Dirty();
                }
            }

            private class Content : TTTPanel
            {
                private readonly List<InspectIcon> _icons = new();
                private int _selectedIconIndex;

                public Content(Panel parent)
                {
                    Parent = parent;
                    _selectedIconIndex = 0;
                }

                public override void Tick()
                {
                    if (_selectedIconIndex < _icons.Count)
                    {
                        for (int i = 0; i < _icons.Count; ++i)
                        {
                            _icons[i].SetClass("selected", i == _selectedIconIndex);
                        }
                    }
                    else
                    {
                        _selectedIconIndex = 0;
                    }
                }

                [Event.BuildInput]
                private void ProcessInspectMenuInput(InputBuilder input)
                {
                    int mouseWheelIndex = input.MouseWheel;
                    if (mouseWheelIndex != 0)
                    {
                        _selectedIconIndex =
                            InventorySelection.NormalizeSlotIndex(-mouseWheelIndex + _selectedIconIndex, _icons.Count - 1);
                    }
                }

                public void SetConfirmationData(ConfirmationData confirmationData, string killerWeapon, string[] perks)
                {
                    _icons.ForEach((i) => i.Delete(true));
                    _icons.Clear();

                    _icons.Add(new InspectIcon(this, "/ui/inspectmenu/time.png"));

                    if (confirmationData.Headshot)
                    {
                        _icons.Add(new InspectIcon(this, "/ui/inspectmenu/headshot.png"));
                    }

                    _icons.Add(!confirmationData.Suicide
                        ? new InspectIcon(this, "/ui/inspectmenu/distance.png")
                        : new InspectIcon(this, ""));

                    if (!String.IsNullOrEmpty(killerWeapon))
                    {
                        _icons.Add(new InspectIcon(this, $"/ui/weapons/{killerWeapon}.png"));
                    }

                    if (perks != null)
                    {
                        foreach (string perkName in perks)
                        {
                            _icons.Add(new InspectIcon(this, $"/ui/weapons/{perkName}.png"));
                        }
                    }
                }
            }

            private class InspectIcon : TTTPanel
            {
                public InspectIcon(Panel parent, string imagePath)
                {
                    Parent = parent;
                    _ = new ImageWrapper(this, imagePath);
                }
            }

            private class ImageWrapper : TTTPanel
            {
                public ImageWrapper(Panel parent, string imagePath)
                {
                    Parent = parent;
                    Add.Image(imagePath, "avatar");
                }
            }
        }

        private class ConfirmCorpsePanel : TTTPanel
        {
            public ConfirmCorpsePanel(Panel parent)
            {
                Parent = parent;
                Add.Label("Press E to confirm corpse");
            }
        }
    }
}
