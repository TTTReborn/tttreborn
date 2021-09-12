using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class QuickShop
    {
        private class Content : TTTPanel
        {
            private readonly List<ItemPanel> _itemPanels = new();

            private readonly Panel _wrapper;

            public Content(Panel parent)
            {
                Parent = parent;

                _wrapper = Add.Panel("wrapper");

                Reload();
            }

            public void Reload()
            {
                _wrapper.DeleteChildren(true);

                if (Local.Pawn is not TTTPlayer player)
                {
                    return;
                }

                Shop shop = player.Shop;

                if (shop == null)
                {
                    return;
                }

                foreach (ShopItemData shopItemData in shop.Items)
                {
                    if (_selectedItemData == null)
                    {
                        _selectedItemData = shopItemData;
                    }

                    AddItem(shopItemData);
                }
            }

            public void Update()
            {
                foreach (ItemPanel itemPanel in _itemPanels)
                {
                    itemPanel.Update();
                }
            }

            public void AddItem(ShopItemData itemData)
            {
                ItemPanel itemPanel = new(_wrapper);
                itemPanel.SetItem(itemData);

                itemPanel.AddEventListener("onclick", () =>
                {
                    if (itemPanel.IsDisabled)
                    {
                        return;
                    }

                    _selectedItemData = itemData;

                    Update();
                });

                _itemPanels.Add(itemPanel);
            }

            private class ItemPanel : TTTPanel
            {
                private ShopItemData _buyableItemData;

                private readonly Panel _iconPanel;

                public Panel PriceHolder;
                public Label DollarSignLabel;

                private readonly Label _priceLabel;

                public bool IsDisabled = false;

                public ItemPanel(Panel parent)
                {
                    Parent = parent;

                    _iconPanel = Add.Panel("icon");
                    PriceHolder = Add.Panel("priceholder");
                    DollarSignLabel = PriceHolder.Add.Label("$", "dollarsign");
                    _priceLabel = PriceHolder.Add.Label("", "price");
                }

                public void SetItem(ShopItemData buyableItemData)
                {
                    _buyableItemData = buyableItemData;

                    _priceLabel.Text = $"{buyableItemData.Price}";

                    _iconPanel.Style.Background = new PanelBackground
                    {
                        Texture = Texture.Load($"/ui/weapons/{buyableItemData.Name}.png")
                    };
                    _iconPanel.Style.Dirty();
                }

                public void Update()
                {
                    IsDisabled = (Local.Pawn as TTTPlayer).CanBuy(_buyableItemData) != BuyError.None;

                    SetClass("disabled", IsDisabled);
                    SetClass("active", _selectedItemData?.Name == _buyableItemData?.Name);
                }
            }
        }
    }
}
