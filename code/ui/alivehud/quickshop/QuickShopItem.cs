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

            Texture icon = null;

            if (shopItemData.Type.IsSubclassOf(typeof(TTTWeapon)))
            {
                icon = Texture.Load($"/ui/weapons/{shopItemData.Name}.png", false);
            }
            else if (shopItemData.Type.IsSubclassOf(typeof(TTTPerk)))
            {
                icon = Texture.Load($"/ui/perks/{shopItemData.Name}.png", false);
            }

            _itemIcon.Style.BackgroundImage = icon ?? Texture.Load($"/ui/none.png");
        }

        public void Update()
        {
            IsDisabled = (Local.Pawn as TTTPlayer).CanBuy(ItemData) != BuyError.None;
            Enabled = !IsDisabled;
        }
    }
}
