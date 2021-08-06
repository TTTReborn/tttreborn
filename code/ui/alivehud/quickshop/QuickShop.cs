using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class QuickShop : TTTPanel
    {
        private static ShopItemData? _selectedItemData;
        private static bool _isOpen = false;

        private Header _header;
        private Footer _footer;
        private bool _wasOpened = false;
        private readonly Content _content;

        public QuickShop()
        {
            StyleSheet.Load("/ui/alivehud/quickshop/QuickShop.scss");

            _header = new Header(this);
            _content = new Content(this);
            _footer = new Footer(this);
        }

        public void Update()
        {
            _content.Update();
        }

        public override void Tick()
        {
            base.Tick();

            _wasOpened = _isOpen;
            _isOpen = !HasClass("hide");

            Update();

            IsShowing = Input.Down(InputButton.Menu) && (Local.Pawn as TTTPlayer).Role.CanBuy();
        }

        private class Header : TTTPanel
        {
            public Panel PriceHolder { get; set; }
            public Label DollarSignLabel;
            private Label _titleLabel;
            private readonly Label _creditsLabel;

            public Header(Panel parent)
            {
                Parent = parent;

                _titleLabel = Add.Label("Shop", "title");
                PriceHolder = Add.Panel("priceholder");
                DollarSignLabel = PriceHolder.Add.Label("$", "dollarsign");
                _creditsLabel = PriceHolder.Add.Label("0", "credits");
            }

            public override void Tick()
            {
                _creditsLabel.Text = $"{(Local.Pawn as TTTPlayer).Credits}";
            }
        }

        private class Content : TTTPanel
        {
            private readonly List<ItemPanel> _itemPanels = new();

            private readonly Panel _wrapper;

            public Content(Panel parent)
            {
                Parent = parent;

                _wrapper = Add.Panel("wrapper");

                foreach (Type type in Globals.Utils.GetTypes<IBuyableItem>())
                {
                    IBuyableItem item = Globals.Utils.GetObjectByType<IBuyableItem>(type);
                    ShopItemData itemData = item.CreateItemData();

                    item.Delete();

                    if (_selectedItemData == null)
                    {
                        _selectedItemData = itemData;
                    }

                    AddItem(itemData);
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
                ItemPanel itemPanel = new ItemPanel(_wrapper);
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
                private ShopItemData? _buyableItemData;

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

        private class Footer : TTTPanel
        {
            private Description _description;
            private BuyArea _buyArea;
            private ShopItemData? _currentItemData;

            public Footer(Panel parent)
            {
                Parent = parent;

                _description = new Description(this);
                _buyArea = new BuyArea(this);
            }

            private class Description : TTTPanel
            {
                public Label EquipmentLabel;
                public Label DescriptionLabel;
                public ShopItemData? ItemData;

                public Description(Panel parent)
                {
                    Parent = parent;

                    EquipmentLabel = Add.Label("ItemName", "equipment");
                    DescriptionLabel = Add.Label("Some item description...", "description");
                }

                public void SetItem(ShopItemData? itemData)
                {
                    ItemData = itemData;

                    EquipmentLabel.Text = itemData?.Name;
                    DescriptionLabel.Text = itemData?.Description ?? "";
                }
            }

            private class BuyArea : TTTPanel
            {
                public Panel PriceHolder;
                public Label DollarSignLabel;
                public Label PriceLabel;
                public Button BuyButton;
                public ShopItemData? ItemData;

                public BuyArea(Panel parent)
                {
                    Parent = parent;
                    PriceHolder = Add.Panel("priceholder");
                    DollarSignLabel = PriceHolder.Add.Label("$", "dollarsign");
                    PriceLabel = PriceHolder.Add.Label("100", "price");

                    BuyButton = Add.Button("Buy", "buyButton");
                    BuyButton.AddEventListener("onclick", () =>
                    {
                        if (_selectedItemData?.IsBuyable(Local.Pawn as TTTPlayer) ?? false)
                        {
                            ConsoleSystem.Run("ttt_requestitem", ItemData?.Name);
                        }
                    });
                }

                public void SetItem(ShopItemData? itemData)
                {
                    ItemData = itemData;
                    PriceLabel.Text = itemData?.Price.ToString();
                }
            }

            public override void Tick()
            {
                base.Tick();

                if (_currentItemData?.Name == _selectedItemData?.Name)
                {
                    return;
                }

                _currentItemData = _selectedItemData;

                _description.SetItem(_currentItemData);
                _buyArea.SetItem(_currentItemData);
            }
        }
    }
}
