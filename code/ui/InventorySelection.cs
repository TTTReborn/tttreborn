using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class InventorySelection : Panel
    {
        public InventorySelection()
        {
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

            Inventory inventory = player.Inventory as Inventory;
            ICarriableItem activeItem = player.ActiveChild as ICarriableItem;
            foreach (Panel child in Children)
            {
                if (child is InventorySlot slot)
                {
                    slot.SetClass("active", slot.Carriable.Name == activeItem?.Name);
                    if (slot.Carriable is TTTWeapon weapon && weapon.HoldType != HoldType.Melee)
                    {
                        slot.UpdateAmmo(FormatAmmo(weapon, inventory));
                    }
                }
            }
        }

        [Event("tttreborn.player.spawned")]
        private void OnPlayerSpawned(TTTPlayer player)
        {
            DeleteChildren();
        }

        [Event("tttreborn.player.carriableitem.pickup")]
        private void OnCarriableItemPickup(ICarriableItem carriable)
        {
            if (Local.Pawn is not TTTPlayer player && carriable != null)
            {
                return;
            }

            AddChild(new InventorySlot(this, carriable));
            SortChildren((p1, p2) =>
            {
                InventorySlot s1 = p1 as InventorySlot;
                InventorySlot s2 = p2 as InventorySlot;

                int result = s1.Carriable.HoldType.CompareTo(s2.Carriable.HoldType);
                return result != 0 ? result : String.Compare(s1.Carriable.Name, s2.Carriable.Name, StringComparison.Ordinal);
            });
        }

        [Event("tttreborn.player.carriableitem.drop")]
        private void OnCarriableItemDrop(ICarriableItem carriable)
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            foreach (Panel child in Children)
            {
                if (child is InventorySlot slot)
                {
                    if (slot.Carriable.Name == carriable.Name)
                    {
                        child.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// IClientInput implementation, calls during the client input build.
        /// You can both read and write to input, to affect what happens down the line.
        /// </summary>
        [Event.BuildInput]
        private void ProcessClientInventorySelectionInput(InputBuilder input)
        {
            if (_children == null || _children.Count == 0)
            {
                return;
            }

            ICarriableItem activeCarriable = Local.Pawn.ActiveChild as ICarriableItem;

            int keyboardIndexPressed = GetKeyboardNumberPressed(input);
            if (keyboardIndexPressed != 0)
            {
                // Find all weapons of hold type that equals the number the user pressed on their keyboard.
                List<InventorySlot> weaponsOfHoldType = _children.ConvertAll(p => p as InventorySlot).Where(
                    (slot) => (int) slot.Carriable.HoldType == keyboardIndexPressed).ToList();

                // Check to see if a user is currently holding a weapon which is of the same hold type.
                bool isActiveCarriableOfWeaponType = weaponsOfHoldType.Any((slot) =>
                    slot.Carriable.Name == activeCarriable?.Name);

                if (activeCarriable == null || !isActiveCarriableOfWeaponType)
                {
                    // The user isn't holding a weapon, or is holding a weapon of a different hold type.
                    // We can just select the first weapon.
                    input.ActiveChild = weaponsOfHoldType.FirstOrDefault()?.Carriable as Entity;
                }
                else
                {
                    // The user is holding a weapon of the currently selected hold type.
                    // Increment the index using GetNextWeaponIndex.
                    int activeCarriableIndex = weaponsOfHoldType.FindIndex((slot) => slot.Carriable.Name == activeCarriable?.Name);
                    input.ActiveChild = weaponsOfHoldType[GetNextWeaponIndex(activeCarriableIndex, weaponsOfHoldType.Count)].Carriable as Entity;
                }
            }

            int mouseWheelIndex = Input.MouseWheel;
            if (mouseWheelIndex != 0)
            {
                int activeCarriableIndex = _children.FindIndex((p) =>
                    p is InventorySlot slot && slot.Carriable.Name == activeCarriable?.Name);

                input.ActiveChild = (_children[NormalizeSlotIndex(mouseWheelIndex + activeCarriableIndex, _children.Count - 1)]
                    as InventorySlot)?.Carriable as Entity;
            }
        }

        // Keyboard selection can only increment the index by 1.
        private int GetNextWeaponIndex(int index, int count)
        {
            index += 1;
            return NormalizeSlotIndex(index, count - 1);
        }

        private int NormalizeSlotIndex(int index, int maxIndex)
        {
            return index > maxIndex ? 0 : index < 0 ? maxIndex : index;
        }

        private int GetKeyboardNumberPressed(InputBuilder input)
        {
            if (input.Pressed(InputButton.Slot1)) return 1;
            if (input.Pressed(InputButton.Slot2)) return 2;
            if (input.Pressed(InputButton.Slot3)) return 3;
            if (input.Pressed(InputButton.Slot4)) return 4;
            if (input.Pressed(InputButton.Slot5)) return 5;

            return 0;
        }

        private static string FormatAmmo(TTTWeapon weapon, Inventory inventory)
        {
            return $"{weapon.AmmoClip} + {(inventory.Ammo.Count(weapon.AmmoType))}";
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
    }
}
