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
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class QuickShopItem : Panel
    {
        public ShopItemData ItemData;
        public bool IsDisabled = false;

        private Panel _itemIcon;
        private TranslationLabel _itemNameLabel;
        private Label _itemPriceLabel;

        public QuickShopItem(Sandbox.UI.Panel parent) : base(parent)
        {
            AddClass("rounded");
            AddClass("text-shadow");
            AddClass("background-color-secondary");

            _itemPriceLabel = Add.Label();
            _itemPriceLabel.AddClass("item-price-label");
            _itemPriceLabel.AddClass("text-color-info");

            _itemIcon = new Panel(this);
            _itemIcon.AddClass("item-icon");

            _itemNameLabel = Add.TranslationLabel();
            _itemNameLabel.AddClass("item-name-label");
        }

        public void SetItem(ShopItemData shopItemData)
        {
            ItemData = shopItemData;

            _itemNameLabel.SetTranslation(shopItemData.Name.ToUpper());
            _itemPriceLabel.Text = $"${shopItemData.Price}";

            _itemIcon.Style.BackgroundImage = Texture.Load($"/ui/icons/{shopItemData.Name}.png", false) ?? Texture.Load($"/ui/none.png");
        }

        public void Update()
        {
            IsDisabled = (Local.Pawn as TTTPlayer).CanBuy(ItemData) != BuyError.None;
            Enabled = !IsDisabled;
        }
    }
}
