// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class Effect : Panel
    {
        public IItem Item
        {
            get
            {
                return _item;
            }
            private set
            {
                _item = value;

                _nameLabel.SetTranslation(_item?.LibraryName.ToUpper() ?? "");
                _effectImage.Texture = (_item != null ? Texture.Load($"/ui/weapons/{_item.LibraryName}.png", false) : null);

                if (_effectImage.Texture == null)
                {
                    _effectImage.Texture = Texture.Load($"/ui/none.png");
                }

                if (_item is TTTCountdownPerk countdownPerk)
                {
                    ActivateCountdown();
                }
                else
                {
                    _countdownLabel?.Delete();
                }
            }
        }

        private IItem _item;
        private readonly TranslationLabel _nameLabel;
        private readonly Panel _effectIconPanel;
        private readonly Image _effectImage;
        private Label _countdownLabel;

        public Effect(Sandbox.UI.Panel parent, IItem effect) : base(parent)
        {
            Parent = parent;

            AddClass("text-shadow");

            _effectIconPanel = new Panel(this);
            _effectIconPanel.AddClass("effect-icon-panel");

            _effectImage = _effectIconPanel.Add.Image();
            _effectImage.AddClass("effect-image");

            _nameLabel = Add.TranslationLabel();
            _nameLabel.AddClass("name-label");

            Item = effect;
        }

        private void ActivateCountdown()
        {
            _countdownLabel = _effectIconPanel.Add.Label();
            _countdownLabel.AddClass("countdown");
            _countdownLabel.AddClass("centered");
            _countdownLabel.AddClass("text-shadow");
        }

        public override void Tick()
        {
            base.Tick();

            if (_countdownLabel != null && Item is TTTCountdownPerk countdownPerk)
            {
                int currentCountdown = (countdownPerk.Countdown - countdownPerk.LastCountdown).CeilToInt();

                if (currentCountdown == countdownPerk.Countdown.CeilToInt() || currentCountdown == 0)
                {
                    _effectImage.SetClass("cooldown", false);
                    _countdownLabel.Text = "";
                }
                else
                {
                    _effectImage.SetClass("cooldown", true);
                    _countdownLabel.Text = $"{currentCountdown:n0}";
                }
            }
        }
    }
}
