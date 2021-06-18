using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class QuickShop : Panel
    {
        public static IBuyableItem SelectedItem;

        private Header header;
        private Content content;
        private Footer footer;

        public QuickShop()
        {
            StyleSheet.Load("/ui/QuickShop.scss");

            header = new Header(this);
            content = new Content(this);
            footer = new Footer(this);
        }

        public override void Tick()
        {
            base.Tick();

            SetClass("hide", !Input.Down(InputButton.Menu));
        }

        private class Header : Panel
        {
            public Label TitleLabel { get; set; }
            public Label CreditsLabel { get; set; }

            public Header(Panel parent)
            {
                Parent = parent;

                TitleLabel = Add.Label("Shop", "title");
                CreditsLabel = Add.Label("$ 0", "credits");
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

                foreach (Type type in Library.GetAll<TTTWeapon>())
                {
                    if (type.IsAbstract || type.ContainsGenericParameters)
                    {
                        continue;
                    }

                    AddItem(Library.Create<TTTWeapon>(type));
                }
            }

            public void AddItem(IBuyableItem buyableItem)
            {
                ItemPanel itemPanel = new ItemPanel(wrapper);
                itemPanel.SetItem(buyableItem);

                itemPanel.AddEvent("onclick", () => {
                    SelectedItem = buyableItem;
                });

                itemPanels.Add(itemPanel);
            }

            private class ItemPanel : Panel
            {
                private IBuyableItem buyableItem;

                public Panel ImagePanel;

                public Label PriceLabel;

                public ItemPanel(Panel parent)
                {
                    Parent = parent;

                    ImagePanel = Add.Panel("image");
                    PriceLabel = Add.Label("", "price");
                }

                public void SetItem(IBuyableItem buyableItem)
                {
                    this.buyableItem = buyableItem;

                    ImagePanel.Add.Label(buyableItem.GetName(), "name");
                    PriceLabel.Text = $"$ {buyableItem.GetPrice()}";
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
                        // request server buy stuff
                    });
                }

                public void SetItem(IBuyableItem item)
                {
                    Item = item;
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
