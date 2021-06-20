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
        public static IBuyableItem SelectedItem;
        public static bool IsOpen = false;

        private Header header;
        private Content content;
        private Footer footer;
        private bool wasOpened = false;

        public QuickShop()
        {
            StyleSheet.Load("/ui/QuickShop.scss");

            header = new Header(this);
            content = new Content(this);
            footer = new Footer(this);
        }

        public void Update()
        {
            content.Update();
        }

        public override void Tick()
        {
            base.Tick();

            wasOpened = IsOpen;
            IsOpen = !HasClass("hide");

            Update();

            SetClass("hide", !Input.Down(InputButton.Menu) || !(Local.Pawn as TTTPlayer).Role.CanBuy());
        }

        private class Header : Panel
        {
            public Panel PriceHolder { get; set; }
            public Label TitleLabel { get; set; }
            public Label DollarSignLabel { get; set; }
            public Label CreditsLabel { get; set; }

            public Header(Panel parent)
            {
                Parent = parent;

                TitleLabel = Add.Label("Shop", "title");
                PriceHolder = Add.Panel("priceholder");
                DollarSignLabel = PriceHolder.Add.Label("$", "dollarsign");
                CreditsLabel =  PriceHolder.Add.Label("0", "credits");
            }

            public override void Tick()
            {
                CreditsLabel.Text = $"{(Local.Pawn as TTTPlayer).Credits}";
            }
        }

        private class Content : Panel
        {
            private List<ItemPanel> itemPanels = new();

            private Panel wrapper;

            public Content(Panel parent)
            {
                Parent = parent;

                wrapper = Add.Panel("wrapper");

                foreach (Type type in Library.GetAll<IBuyableItem>())
                {
                    if (type.IsAbstract || type.ContainsGenericParameters)
                    {
                        continue;
                    }

                    IBuyableItem item = Library.Create<IBuyableItem>(type);

                    if (SelectedItem == null)
                    {
                        SelectedItem = item;
                    }

                    AddItem(item);
                }
            }

            public void Update()
            {
                foreach(ItemPanel itemPanel in itemPanels)
                {
                    itemPanel.Update();
                }
            }

            public void AddItem(IBuyableItem buyableItem)
            {
                ItemPanel itemPanel = new ItemPanel(wrapper);
                itemPanel.SetItem(buyableItem);

                itemPanel.AddEvent("onclick", () => {
                    if (itemPanel.IsDisabled)
                    {
                        return;
                    }

                    SelectedItem = buyableItem;

                    Update();
                });

                itemPanels.Add(itemPanel);
            }

            private class ItemPanel : Panel
            {
                private IBuyableItem buyableItem;

                public Panel IconPanel;

                public Panel PriceHolder;
                public Label DollarSignLabel;

                public Label PriceLabel;

                public bool IsDisabled = false;

                public ItemPanel(Panel parent)
                {
                    Parent = parent;

                    IconPanel = Add.Panel("icon");
                    PriceHolder = Add.Panel("priceholder");
                    DollarSignLabel = PriceHolder.Add.Label("$", "dollarsign");
                    PriceLabel = PriceHolder.Add.Label("", "price");
                }

                public void SetItem(IBuyableItem buyableItem)
                {
                    this.buyableItem = buyableItem;

                    PriceLabel.Text = $"{buyableItem.GetPrice()}";

                    IconPanel.Style.Background = new PanelBackground{
                        Texture = Texture.Load($"/ui/weapons/{buyableItem.GetName()}.png")
                    };
                    IconPanel.Style.Dirty();
                }

                public void Update()
                {
                    IsDisabled = (Local.Pawn as TTTPlayer).CanBuy(buyableItem) != BuyError.None;


                    SetClass("disabled", IsDisabled);
                    SetClass("active", SelectedItem == buyableItem);
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
                public Panel PriceHolder;
                public Label DollarSignLabel;
                public Label PriceLabel;
                public Button BuyButton;
                public IBuyableItem Item;

                public BuyArea(Panel parent)
                {
                    Parent = parent;
                    PriceHolder = Add.Panel("priceholder");
                    DollarSignLabel = PriceHolder.Add.Label("$","dollarsign");
                    PriceLabel = PriceHolder.Add.Label("100", "price");

                    BuyButton = Add.Button("Buy", "buyButton");
                    BuyButton.AddEvent("onclick", () => {
                        if (SelectedItem.IsBuyable(Local.Pawn as TTTPlayer))
                        {
                            ConsoleSystem.Run($"requestitem", Item.GetName());
                        }
                    });
                }

                public void SetItem(IBuyableItem item)
                {
                    Item = item;
                    PriceLabel.Text = item.GetPrice().ToString();
                }
            }

            public override void Tick()
            {
                if (currentBuyableItem == SelectedItem)
                {
                    return;
                }

                currentBuyableItem = SelectedItem;

                description.SetItem(currentBuyableItem);
                buyArea.SetItem(currentBuyableItem);
            }
        }
    }
}
