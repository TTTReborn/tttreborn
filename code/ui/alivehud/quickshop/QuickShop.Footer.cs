using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class QuickShop
    {
        private class Footer : TTTPanel
        {
            private Description _description;
            private BuyArea _buyArea;
            private ShopItemData? _currentItemData;

            public Footer(Panel parent)
            {
                Parent = parent;

                _description = new(this);
                _buyArea = new(this);
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
