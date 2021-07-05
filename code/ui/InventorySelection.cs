using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class InventorySelection : Panel
    {
        private readonly Dictionary<string, InventorySlot> _inventorySlots;

        public InventorySelection()
        {
            _inventorySlots = new Dictionary<string, InventorySlot>();
            StyleSheet.Load("/ui/InventorySelection.scss");

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            Inventory inventory = player.Inventory as Inventory;

            foreach (Entity entity in inventory.List)
            {
                if (entity is ICarriableItem carriableItem)
                {
                    OnCarriableItemPickup(carriableItem);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            ICarriableItem activeItem = player.ActiveChild as ICarriableItem;
            foreach ((_, InventorySlot value) in _inventorySlots)
            {
                if (value.Carriable is TTTWeapon weapon && weapon.HoldType != HoldType.Melee)
                {
                    value.UpdateAmmo(FormatAmmo(weapon, player.Inventory as Inventory));
                }

                value.SetClass("active", value.Carriable.Name == activeItem?.Name);
            }

            SetClass("hide", _inventorySlots.Count == 0);
        }

        [Event("tttreborn.player.carriableitem.pickup")]
        private void OnCarriableItemPickup(ICarriableItem carriable)
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (_inventorySlots.ContainsKey(carriable.Name))
            {
                return;
            }

            _inventorySlots.Add(carriable.Name, new InventorySlot(this, carriable));

            SortChildren<InventorySlot>((inventorySlot) => (int) inventorySlot.Carriable.HoldType);
        }

        [Event("tttreborn.player.carriableitem.drop")]
        private void OnCarriableItemDrop(ICarriableItem carriable)
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (!_inventorySlots.TryGetValue(carriable.Name, out InventorySlot slot))
            {
                return;
            }

            slot.Delete();
            _inventorySlots.Remove(carriable.Name);
        }

        /// <summary>
        /// IClientInput implementation, calls during the client input build.
        /// You can both read and write to input, to affect what happens down the line.
        /// </summary>
        [Event.BuildInput]
        private void ProcessClientInventorySelectionInput(InputBuilder input)
        {
            Inventory inventory = Local.Pawn.Inventory as Inventory;
            // TODO: See if we can remove this sort operation, we already do one at the end of ItemPickup.
            inventory.List.Sort((Entity carr1, Entity carr2) => (carr1 as ICarriableItem).HoldType.CompareTo((carr2 as ICarriableItem).HoldType));

            if (inventory.Count() == 0)
            {
                return;
            }

            int selectedInventoryIndex = SlotPressInput(input);

            // no button pressed, but maybe scrolled
            if (selectedInventoryIndex == 0)
            {
                if (input.MouseWheel != 0)
                {
                    int nextSlot = inventory.List.IndexOf(Local.Pawn.ActiveChild) - input.MouseWheel;
                    nextSlot = inventory.NormalizeSlotIndex(nextSlot, inventory.Count() - 1);

                    input.ActiveChild = inventory.GetSlot(nextSlot);

                    input.MouseWheel = 0;
                }

                return;
            }

            // corresponding key pressed
            int nextInventorySlot = inventory.GetNextSlot((HoldType) selectedInventoryIndex);

            if (nextInventorySlot != -1)
            {
                input.ActiveChild = inventory.GetSlot(nextInventorySlot);
            }
        }

        // TODO: Handle mouse wheel, and additional number keys.
        private int SlotPressInput(InputBuilder input)
        {
            if (input.Pressed(InputButton.Slot1)) return 1;
            if (input.Pressed(InputButton.Slot2)) return 2;
            if (input.Pressed(InputButton.Slot3)) return 3;
            if (input.Pressed(InputButton.Slot4)) return 4;
            if (input.Pressed(InputButton.Slot5)) return 5;

            return 0;
        }

        private class InventorySlot : Panel
        {
            public ICarriableItem Carriable { get; private set; }
            private readonly Label _ammoLabel;
            private Label _slotLabel;
            private Label _carriableLabel;

            public InventorySlot(Panel parent, ICarriableItem carriable)
            {
                Parent = parent;
                Carriable = carriable;

                _slotLabel = Add.Label(((int) carriable.HoldType).ToString(), "slotlabel");
                _carriableLabel = Add.Label(carriable.Name, "carriablelabel");

                if (carriable.HoldType != HoldType.Melee && carriable is TTTWeapon weapon)
                {
                    _ammoLabel = Add.Label(FormatAmmo(weapon, (Local.Pawn as TTTPlayer).Inventory as Inventory), "ammolabel");
                }
            }

            public void UpdateAmmo(string ammoText)
            {
                _ammoLabel.Text = ammoText;
            }
        }

        private static string FormatAmmo(TTTWeapon weapon, Inventory inventory)
        {
            return $"{weapon.AmmoClip} + {(inventory.Ammo.Count(weapon.AmmoType))}";
        }
    }
}
