using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class QuickShopItem : Panel
    {
        public ShopItemData? ItemData;

        private Panel _itemIcon;
        private Label _itemNameLabel;
        private Label _itemPriceLabel;

        public bool IsDisabled = false;

        public QuickShopItem(Sandbox.UI.Panel parent) : base(parent)
        {
            Parent = parent;

            AddClass("rounded");
            AddClass("text-shadow");
            AddClass("background-color-secondary");

            _itemNameLabel = Add.Label();

            _itemIcon = new Panel(this);
            _itemIcon.AddClass("item-icon");

            _itemPriceLabel = Add.Label();
            _itemPriceLabel.AddClass("item-price-label");
            _itemPriceLabel.AddClass("text-color-info");
        }

        public void SetItem(ShopItemData buyableItemData)
        {
            ItemData = buyableItemData;

            _itemNameLabel.Text = $"{buyableItemData.Name}";
            _itemPriceLabel.Text = $"${buyableItemData.Price}";

            var icon = Texture.Load($"/ui/weapons/{buyableItemData.Name}.png");
            if (icon == null)
            {
                icon = Texture.Load($"/ui/none.png");
            }

            _itemIcon.Style.Background = new PanelBackground
            {
                Texture = icon
            };

            _itemIcon.Style.Dirty();
        }

        public void Update()
        {
            IsDisabled = (Local.Pawn as TTTPlayer).CanBuy(ItemData) != BuyError.None;
            Enabled = !IsDisabled;
        }
    }
}
