using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class QuickShop : Panel
    {
        private static IBuyableItem _selectedItem;
        private static bool _isOpen = false;

        private Header _header;
        private Footer _footer;
        private bool _wasOpened = false;
        private readonly Content _content;

        public QuickShop()
        {
            StyleSheet.Load("/ui/QuickShop.scss");

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

            SetClass("hide", !Input.Down(InputButton.Menu) || !(Local.Pawn as TTTPlayer).Role.CanBuy());
        }

        private class Header : Panel
        {
            private Label _titleLabel;
            private readonly Label _creditsLabel;

            public Header(Panel parent)
            {
                Parent = parent;

                _titleLabel = Add.Label("Shop", "title");
                _creditsLabel = Add.Label("$ 0", "credits");
            }

            public override void Tick()
            {
                _creditsLabel.Text = $"$ {(Local.Pawn as TTTPlayer).Credits}";
            }
        }

        private class Content : Panel
        {
            private readonly List<ItemPanel> _itemPanels = new();

            private readonly Panel _wrapper;

            public Content(Panel parent)
            {
                Parent = parent;

                _wrapper = Add.Panel("wrapper");

                foreach (Type type in Library.GetAll<IBuyableItem>())
                {
                    if (type.IsAbstract || type.ContainsGenericParameters)
                    {
                        continue;
                    }

                    IBuyableItem item = Library.Create<IBuyableItem>(type);

                    if (_selectedItem == null)
                    {
                        _selectedItem = item;
                    }

                    AddItem(item);
                }
            }

            public void Update()
            {
                foreach(ItemPanel itemPanel in _itemPanels)
                {
                    itemPanel.Update();
                }
            }

            public void AddItem(IBuyableItem buyableItem)
            {
                ItemPanel itemPanel = new ItemPanel(_wrapper);
                itemPanel.SetItem(buyableItem);

                itemPanel.AddEvent("onclick", () => {
                    if (itemPanel.IsDisabled)
                    {
                        return;
                    }

                    _selectedItem = buyableItem;

                    Update();
                });

                _itemPanels.Add(itemPanel);
            }

            private class ItemPanel : Panel
            {
                private IBuyableItem _buyableItem;

                private readonly Panel _iconPanel;

                private readonly Label _priceLabel;

                public bool IsDisabled = false;

                public ItemPanel(Panel parent)
                {
                    Parent = parent;

                    _iconPanel = Add.Panel("icon");
                    _priceLabel = Add.Label("", "price");
                }

                public void SetItem(IBuyableItem buyableItem)
                {
                    this._buyableItem = buyableItem;

                    _priceLabel.Text = $"$ {buyableItem.GetPrice()}";

                    _iconPanel.Style.Background = new PanelBackground{
                        Texture = Texture.Load($"/ui/weapons/{buyableItem.GetName()}.png")
                    };
                    _iconPanel.Style.Dirty();
                }

                public void Update()
                {
                    IsDisabled = (Local.Pawn as TTTPlayer).CanBuy(_buyableItem) != BuyError.None;

                    SetClass("disabled", IsDisabled);
                    SetClass("active", _selectedItem == _buyableItem);
                }
            }
        }

        private class Footer : Panel
        {
            private Description description;
            private BuyArea buyArea;
            private IBuyableItem currentBuyableItem;

            public Footer(Panel parent)
            {
                Parent = parent;

                description = new Description(this);
                buyArea = new BuyArea(this);
            }

            private class Description : Panel
            {
                public Label EquipmentLabel;
                public Label DescriptionLabel;
                public IBuyableItem Item;

                public Description(Panel parent)
                {
                    Parent = parent;

                    EquipmentLabel = Add.Label("ItemName", "equipment");
                    DescriptionLabel = Add.Label("Some item description...", "description");
                }

                public void SetItem(IBuyableItem item)
                {
                    Item = item;

                    EquipmentLabel.Text = Item.GetName();

                    if (item is TTTWeapon weapon)
                    {
                        DescriptionLabel.Text = $"Slot: {(int) weapon.WeaponType}";
                    }
                }
            }

            private class BuyArea : Panel
            {
                public Label PriceLabel;
                public Button BuyButton;
                public IBuyableItem Item;

                public BuyArea(Panel parent)
                {
                    Parent = parent;

                    PriceLabel = Add.Label("$ 200", "price");

                    BuyButton = Add.Button("Buy", "buyButton");
                    BuyButton.AddEvent("onclick", () => {
                        if (_selectedItem.IsBuyable(Local.Pawn as TTTPlayer))
                        {
                            ConsoleSystem.Run($"requestitem", Item.GetName());
                        }
                    });
                }

                public void SetItem(IBuyableItem item)
                {
                    Item = item;
                }
            }

            public override void Tick()
            {
                if (currentBuyableItem == _selectedItem)
                {
                    return;
                }

                currentBuyableItem = _selectedItem;

                description.SetItem(currentBuyableItem);
                buyArea.SetItem(currentBuyableItem);
            }
        }
    }
}
