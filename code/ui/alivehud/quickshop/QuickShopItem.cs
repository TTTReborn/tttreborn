using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;
using TTTReborn.Items;

#pragma warning disable IDE0052

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class QuickShopItem : Panel
    {
        public ShopItemData ItemData;
        public bool IsDisabled = false;

        private Panel ItemIcon { get; set; }
        private TranslationLabel ItemNameLabel { get; set; }
        private string Price { get; set; }

        public void SetItem(ShopItemData shopItemData)
        {
            ItemData = shopItemData;

            ItemNameLabel.UpdateTranslation(new TranslationData(shopItemData.GetTranslationKey("NAME")));
            Price = $"${shopItemData.Price}";

            ItemIcon.Style.BackgroundImage = Texture.Load(FileSystem.Mounted, $"assets/icons/{shopItemData.Name}.png") ?? Texture.Load(FileSystem.Mounted, $"assets/none.png");
        }

        public void Update()
        {
            IsDisabled = (Game.LocalPawn as Player).CanBuy(ItemData) != BuyError.None;

            this.Enabled(!IsDisabled);
        }
    }
}
