using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class QuickShop : Panel
    {
        public static QuickShop Instance;

        public static ShopItemData? _selectedItemData;

        private readonly List<QuickShopItem> _items = new();
        private Panel _backgroundPanel;
        private Panel _quickshopContainer;
        private Label _creditLabel;
        private Panel _itemPanel;
        private Label _itemDescriptionLabel;

        private int _credits = 0;

        public new bool Enabled
        {
            get => base._isEnabled;
            set
            {
                base._isEnabled = value;

                SetClass("fade-in", base._isEnabled);
                _quickshopContainer.SetClass("pop-in", base._isEnabled);
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
            _backgroundPanel.AddClass("centered");
            _backgroundPanel.AddClass("fullscreen");

            _quickshopContainer = new Panel(this);
            _quickshopContainer.AddClass("quickshop-container");

            _creditLabel = _quickshopContainer.Add.Label();
            _creditLabel.AddClass("credit-label");

            _itemPanel = new Panel(_quickshopContainer);
            _itemPanel.AddClass("item-panel");

            _itemDescriptionLabel = _quickshopContainer.Add.Label();
            _itemDescriptionLabel.AddClass("item-description-label");

            PopulateItems();

            Enabled = false;
        }

        public void PopulateItems()
        {
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
                    ConsoleSystem.Run("ttt_requestitem", item.ItemData?.Name);
                }

                Update();
            });

            _items.Add(item);
        }

        public void Update()
        {
            _creditLabel.Text = $"You have ${_credits}";

            foreach (QuickShopItem item in _items)
            {
                item.Update();
            }

            _itemDescriptionLabel.SetClass("fade-in", _selectedItemData != null);

            if (_selectedItemData != null)
            {
                _itemDescriptionLabel.Text = $"The description for {_selectedItemData?.Name} will go here";
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

        public bool CheckAccess()
        {
            return (Local.Pawn as TTTPlayer).Role.CanBuy();
        }
    }
}

namespace TTTReborn.Player
{
    using UI;

    public partial class TTTPlayer
    {
        [ClientCmd("+ttt_quickshop")]
        public static void OpenQuickshop()
        {
            if (QuickShop.Instance == null || !QuickShop.Instance.CheckAccess())
            {
                return;
            }

            QuickShop.Instance.Enabled = true;
            QuickShop.Instance.Update();
        }

        [ClientCmd("-ttt_quickshop")]
        public static void CloseQuickshop()
        {
            if (QuickShop.Instance == null)
            {
                return;
            }

            QuickShop.Instance.Enabled = false;
            QuickShop.Instance.Update();
        }
    }
}
