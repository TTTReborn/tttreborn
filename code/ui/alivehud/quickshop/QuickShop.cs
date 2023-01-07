using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;
using TTTReborn.Items;

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class QuickShop : Panel
    {
        public static QuickShop Instance { get; internal set; }

        public static ShopItemData SelectedItemData { get; set; }

        private readonly List<QuickShopItem> _items = new();
        private Panel QuickshopContainer { get; set; }
        private TranslationLabel CreditsLabel { get; set; }
        private Panel ItemPanel { get; set; }
        private TranslationLabel ItemDescriptionLabel { get; set; }

        private int _credits = 0;

        public bool Enabled
        {
            get => this.IsEnabled();
            set
            {
                this.Enabled(value);

                SetClass("fade-in", this.IsEnabled());
                QuickshopContainer.SetClass("pop-in", this.IsEnabled());
            }
        }

        public QuickShop()
        {
            Instance = this;

            Reload();

            Enabled = false;
        }

        public void Reload()
        {
            ItemPanel?.DeleteChildren(true);

            SelectedItemData = null;

            if (Game.LocalPawn is not Player player)
            {
                return;
            }

            Shop shop = player.Shop;

            if (shop == null)
            {
                return;
            }

            foreach (ShopItemData itemData in shop.Items)
            {
                AddItem(itemData);
            }
        }

        private void AddItem(ShopItemData itemData)
        {
            QuickShopItem item = new();
            item.SetItem(itemData);

            item.AddEventListener("onmouseover", () =>
            {
                SelectedItemData = itemData;

                Update();
            });

            item.AddEventListener("onmouseout", () =>
            {
                SelectedItemData = null;

                Update();
            });

            item.AddEventListener("onclick", () =>
            {
                if (item.IsDisabled)
                {
                    return;
                }

                if (SelectedItemData?.IsBuyable(Game.LocalPawn as Player) ?? false)
                {
                    Player.RequestItem(item.ItemData?.Name);

                    // The item was purchased, let's deselect it from the UI.
                    SelectedItemData = null;
                }

                Update();
            });

            _items.Add(item);
            ItemPanel.AddChild(item);
        }

        public void Update()
        {
            CreditsLabel.UpdateTranslation(new TranslationData("QUICKSHOP.CREDITS.DESCRIPTION", _credits));

            foreach (QuickShopItem item in _items)
            {
                item.Update();
            }

            ItemDescriptionLabel.SetClass("fade-in", SelectedItemData != null);

            if (SelectedItemData != null)
            {
                ItemDescriptionLabel.UpdateTranslation(new TranslationData("QUICKSHOP.ITEM.DESCRIPTION", new TranslationData(SelectedItemData.GetTranslationKey("NAME"))));
            }
        }

        [Event("shop_change")]
        public static void OnShopChanged()
        {
            Instance?.Reload();
        }

        [Event("player_role_select")]
        public static void OnRoleChanged(Player player)
        {
            QuickShop quickShop = Instance;

            if (quickShop != null)
            {
                if (player.Shop == null || !player.Shop.Accessable())
                {
                    quickShop.Enabled = false;
                }
                else if (quickShop.Enabled)
                {
                    quickShop.Update();
                }
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (!Enabled)
            {
                return;
            }

            int newCredits = (Game.LocalPawn as Player).Credits;

            if (_credits != newCredits)
            {
                _credits = newCredits;

                Update();
            }
        }
    }
}

namespace TTTReborn
{
    using TTTReborn.UI;

    public partial class Player
    {
        public static void TickPlayerShop()
        {
            if (QuickShop.Instance == null)
            {
                return;
            }

            if (Input.Released(InputButton.View))
            {
                QuickShop.Instance.Enabled = false;
                QuickShop.Instance.Update();
            }
            else if (Input.Pressed(InputButton.View) && Game.LocalPawn is Player player)
            {
                if (!(player.Shop?.Accessable() ?? false))
                {
                    return;
                }

                QuickShop.Instance.Enabled = true;
                QuickShop.Instance.Update();
            }
        }
    }
}
