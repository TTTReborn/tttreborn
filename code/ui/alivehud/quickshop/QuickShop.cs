using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Events;
using TTTReborn.Globalization;
using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class QuickShop : Panel
    {
        public static QuickShop Instance;

        public static ShopItemData _selectedItemData;

        private readonly List<QuickShopItem> _items = new();
        private Panel _backgroundPanel;
        private Panel _quickshopContainer;
        private TranslationLabel _creditLabel;
        private Panel _itemPanel;
        private TranslationLabel _itemDescriptionLabel;

        private int _credits = 0;

        public bool Enabled
        {
            get => this.IsEnabled();
            set
            {
                this.Enabled(value);

                SetClass("fade-in", this.IsEnabled());
                _quickshopContainer.SetClass("pop-in", this.IsEnabled());
            }
        }

        public QuickShop() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/quickshop/QuickShop.scss");

            AddClass("text-shadow");

            _backgroundPanel = new Panel(this);
            _backgroundPanel.AddClass("background-color-secondary");
            _backgroundPanel.AddClass("opacity-medium");
            _backgroundPanel.AddClass("fullscreen");

            _quickshopContainer = new Panel(this);
            _quickshopContainer.AddClass("quickshop-container");

            _creditLabel = _quickshopContainer.Add.TranslationLabel(new TranslationData());
            _creditLabel.AddClass("credit-label");

            _itemPanel = new Panel(_quickshopContainer);
            _itemPanel.AddClass("item-panel");

            _itemDescriptionLabel = _quickshopContainer.Add.TranslationLabel(new TranslationData());
            _itemDescriptionLabel.AddClass("item-description-label");

            Reload();

            Enabled = false;
        }

        public void Reload()
        {
            _itemPanel?.DeleteChildren(true);

            _selectedItemData = null;

            if (Local.Pawn is not TTTPlayer player)
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
            QuickShopItem item = new(_itemPanel);
            item.SetItem(itemData);

            item.AddEventListener("onmouseover", () =>
            {
                _selectedItemData = itemData;

                Update();
            });

            item.AddEventListener("onmouseout", () =>
            {
                _selectedItemData = null;

                Update();
            });

            item.AddEventListener("onclick", () =>
            {
                if (item.IsDisabled)
                {
                    return;
                }

                if (_selectedItemData?.IsBuyable(Local.Pawn as TTTPlayer) ?? false)
                {
                    TTTPlayer.RequestItem(item.ItemData?.Name);

                    // The item was purchased, let's deselect it from the UI.
                    _selectedItemData = null;
                }

                Update();
            });

            _items.Add(item);
        }

        public void Update()
        {
            _creditLabel.UpdateTranslation(new TranslationData("QUICKSHOP_CREDITS_DESCRIPTION", _credits));

            foreach (QuickShopItem item in _items)
            {
                item.Update();
            }

            _itemDescriptionLabel.SetClass("fade-in", _selectedItemData != null);

            if (_selectedItemData != null)
            {
                _itemDescriptionLabel.UpdateTranslation(new TranslationData("QUICKSHOP_ITEM_DESCRIPTION", new TranslationData(_selectedItemData?.Name.ToUpper())));
            }
        }

        [Event(TTTEvent.Shop.CHANGE)]
        public static void OnShopChanged()
        {
            Instance?.Reload();
        }

        [Event(TTTEvent.Player.Role.SELECT)]
        public static void OnRoleChanged(TTTPlayer player)
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

            int newCredits = (Local.Pawn as TTTPlayer).Credits;

            if (_credits != newCredits)
            {
                _credits = newCredits;

                Update();
            }
        }
    }
}

namespace TTTReborn.Player
{
    using UI;

    public partial class TTTPlayer
    {
        public void TickPlayerShop()
        {
            if (!IsClient || QuickShop.Instance == null)
            {
                return;
            }

            if (Input.Released(InputButton.View))
            {
                QuickShop.Instance.Enabled = false;
                QuickShop.Instance.Update();
            }
            else if (Input.Pressed(InputButton.View) && Local.Pawn is TTTPlayer player)
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
