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

        [Event("tttreborn.player.inventory.clear")]
        private void OnCarriableItemClear()
        {
            DeleteChildren();
        }

        [Event("tttreborn.player.carriableitem.pickup")]
        private void OnCarriableItemPickup(ICarriableItem carriable)
        {
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
            if (Children == null || !Children.Any())
            {
                return;
            }

            List<Panel> childrenList = Children.ToList();

            ICarriableItem activeCarriable = Local.Pawn.ActiveChild as ICarriableItem;

            int keyboardIndexPressed = GetKeyboardNumberPressed(input);
            if (keyboardIndexPressed != 0)
            {
                List<ICarriableItem> weaponsOfHoldTypeSelected = new();
                int activeCarriableOfHoldTypeIndex = -1;

                for (int i = 0; i < childrenList.Count; ++i)
                {
                    if (childrenList[i] is InventorySlot slot)
                    {
                        if ((int) slot.Carriable.HoldType == keyboardIndexPressed)
                        {
                            // Using the keyboard index the user pressed, find all carriables that
                            // have the same hold type as the index.
                            // Ex. "3" pressed, find all carriables with hold type "3".
                            weaponsOfHoldTypeSelected.Add(slot.Carriable);

                            if (slot.Carriable.Name == activeCarriable?.Name)
                            {
                                // If the current active carriable has the same hold type as
                                // the keyboard index the user pressed
                                activeCarriableOfHoldTypeIndex = weaponsOfHoldTypeSelected.Count - 1;
                            }
                        }
                    }
                }

                if (activeCarriable == null || activeCarriableOfHoldTypeIndex == -1)
                {
                    // The user isn't holding an active carriable, or is holding a weapon that has a different
                    // hold type than the one selected using the keyboard. We can just select the first weapon.
                    input.ActiveChild = weaponsOfHoldTypeSelected.FirstOrDefault() as Entity;
                }
                else
                {
                    // The user is holding a weapon that has the same hold type as the keyboard index the user pressed.
                    // Find the next possible weapon within the hold types.
                    input.ActiveChild = weaponsOfHoldTypeSelected[GetNextWeaponIndex(activeCarriableOfHoldTypeIndex, weaponsOfHoldTypeSelected.Count)] as Entity;
                }
            }

            int mouseWheelIndex = Input.MouseWheel;
            if (mouseWheelIndex != 0)
            {
                int activeCarriableIndex = childrenList.FindIndex((p) =>
                    p is InventorySlot slot && slot.Carriable.Name == activeCarriable?.Name);

                int newSelectedIndex = NormalizeSlotIndex(-mouseWheelIndex + activeCarriableIndex, childrenList.Count - 1);
                input.ActiveChild = (childrenList[newSelectedIndex] as InventorySlot)?.Carriable as Entity;
            }
        }

        // Keyboard selection can only increment the index by 1.
        private int GetNextWeaponIndex(int index, int count)
        {
            return NormalizeSlotIndex(index + 1, count - 1);
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
