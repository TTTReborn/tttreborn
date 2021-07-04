using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

// TODO Fix animation on dropping a higher slot carriable entity (instead of deleting and recreating, move and delete InventorySlots in DOM)

namespace TTTReborn.UI
{
    public class InventorySelection : Panel
    {
        private readonly Dictionary<string, InventorySlot> _inventorySlots = new();

        private ICarriableItem _oldActiveInventory;

        public InventorySelection()
        {
            StyleSheet.Load("/ui/InventorySelection.scss");
        }

        // TODO: This method could be made to be event based. Whenever a user pickups a carriable entity, and whenever a user fires.
        // Consider checking out DM98 or Hidden inventory HUD.
        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            // update inventory slots, check for removed inventory etc.
            Inventory inventory = player.Inventory as Inventory;
            int inventoryCount = inventory.Count();
            List<ICarriableItem> newCarriables = new();
            List<InventorySlot> tmpSlots = new();

            for (int i = 0; i < inventoryCount; i++)
            {
                ICarriableItem carriable = inventory.GetSlot(i) as ICarriableItem;

                if (_inventorySlots.TryGetValue(carriable.Name, out InventorySlot inventorySlot))
                {
                    tmpSlots.Add(inventorySlot);
                }
                else
                {
                    newCarriables.Add(carriable);
                }

                // Do not update if we already rebuilding the InventorySlots
                if (newCarriables.Count == 0 && carriable.HoldType != HoldType.Melee && carriable is TTTWeapon weapon)
                {
                    inventorySlot.UpdateAmmo($"{weapon.AmmoClip} + {(player.Inventory as Inventory).Ammo.Count(weapon.AmmoType)}");
                }
            }

            // remove InventorySlots and rebuild (to keep the right order)
            if (newCarriables.Count != 0 || tmpSlots.Count != _inventorySlots.Count)
            {
                foreach (InventorySlot inventorySlot in _inventorySlots.Values)
                {
                    inventorySlot.Delete();
                }

                _inventorySlots.Clear();

                inventory.List.Sort((Entity carr1, Entity carr2) => (carr1 as ICarriableItem).HoldType.CompareTo((carr2 as ICarriableItem).HoldType));

                // rebuild InventorySlots in the right order
                foreach (ICarriableItem carriable in inventory.List)
                {
                    // add in order
                    _inventorySlots.Add(carriable.Name, new InventorySlot(this, carriable));
                }

                // update for dropped active carriable entity maybe
                _oldActiveInventory = null;
            }

            // update current selection
            ICarriableItem activeInventory = player.ActiveChild as ICarriableItem;

            if (_oldActiveInventory != activeInventory && activeInventory != null)
            {
                foreach (InventorySlot inventorySlot in _inventorySlots.Values)
                {
                    inventorySlot.SetClass("active", inventorySlot.CarriableName == activeInventory.Name);
                }

                _oldActiveInventory = activeInventory;
            }

            SetClass("hide", _inventorySlots.Count == 0);
        }

        /// <summary>
        /// IClientInput implementation, calls during the client input build.
        /// You can both read and write to input, to affect what happens down the line.
        /// </summary>
        [Event.BuildInput]
        private void ProcessClientInventorySelectionInput(InputBuilder input)
        {
            Inventory inventory = Local.Pawn.Inventory as Inventory;

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
            public readonly string CarriableName;
            private readonly Label _ammoLabel;
            private Label _slotLabel;
            private Label _carriableLabel;

            public InventorySlot(Panel parent, ICarriableItem carriable)
            {
                Parent = parent;
                CarriableName = carriable.Name;

                _slotLabel = Add.Label(((int) carriable.HoldType).ToString(), "slotlabel");
                _carriableLabel = Add.Label(carriable.Name, "carriablelabel");

                if (carriable.HoldType != HoldType.Melee && carriable is TTTWeapon weapon)
                {
                    _ammoLabel = Add.Label($"{weapon.AmmoClip}/{weapon.ClipSize}", "ammolabel");
                }
            }

            public void UpdateAmmo(string ammoText)
            {
                _ammoLabel.Text = ammoText;
            }
        }
    }
}
