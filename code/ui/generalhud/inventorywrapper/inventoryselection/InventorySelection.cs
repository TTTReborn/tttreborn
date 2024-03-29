using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class InventorySelection : Panel
    {
        private readonly InputButton[] _slotInputButtons = new[]
        {
            InputButton.Slot0,
            InputButton.Slot1,
            InputButton.Slot2,
            InputButton.Slot3,
            InputButton.Slot4,
            InputButton.Slot5,
            InputButton.Slot6,
            InputButton.Slot7,
            InputButton.Slot8,
            InputButton.Slot9
        };

        public InventorySelection() : base()
        {
            StyleSheet.Load("/ui/generalhud/inventorywrapper/inventoryselection/InventorySelection.scss");

            AddClass("opacity-heavy");
            AddClass("text-shadow");
            AddClass("inventory-selection");

            if (Local.Pawn is not Player player)
            {
                return;
            }

            foreach (Entity entity in player.CurrentPlayer.Inventory.List)
            {
                if (entity is ICarriableItem)
                {
                    OnCarriableItemPickup(entity);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not Player player)
            {
                return;
            }

            // Due to S&Box RPC ent syncing bugs, we have to run some checks and fixes
            List<InventorySlot> inventorySlots = new();

            foreach (Panel panel in Children)
            {
                if (panel is InventorySlot inventorySlot)
                {
                    inventorySlots.Add(inventorySlot);
                }
            }

            bool invalidList = player.CurrentPlayer.Inventory.List.Count != inventorySlots.Count;

            if (!invalidList)
            {
                foreach (Entity entity in player.CurrentPlayer.Inventory.List)
                {
                    if (entity is not ICarriableItem carriableItem)
                    {
                        continue;
                    }

                    string entName = carriableItem.Info.LibraryName;
                    bool found = false;

                    foreach (InventorySlot inventorySlot in inventorySlots)
                    {
                        if (inventorySlot.Carriable.Info.LibraryName.Equals(entName))
                        {
                            found = true;

                            break;
                        }
                    }

                    if (!found)
                    {
                        invalidList = true;

                        break;
                    }
                }
            }

            if (invalidList)
            {
                OnCarriableItemClear();

                foreach (Entity entity in player.CurrentPlayer.Inventory.List)
                {
                    if (entity is ICarriableItem)
                    {
                        OnCarriableItemPickup(entity);
                    }
                }
            }

            ICarriableItem activeItem = player.CurrentPlayer.ActiveChild as ICarriableItem;

            bool invalidSlot = false;

            foreach (Panel child in Children)
            {
                if (child is InventorySlot slot)
                {
                    if (slot.Carriable == null)
                    {
                        invalidSlot = true;

                        break;
                    }

                    slot.SetClass("rounded-top", child == Children.First());
                    slot.SetClass("rounded-bottom", child == Children.Last());

                    slot.SlotLabel.SetClass("rounded-top-left", child == Children.First());
                    slot.SlotLabel.SetClass("rounded-bottom-left", child == Children.Last());

                    slot.SetClass("active", slot.Carriable.Info.LibraryName == activeItem?.Info.LibraryName);
                    slot.SetClass("opacity-heavy", slot.Carriable.Info.LibraryName == activeItem?.Info.LibraryName);

                    if (slot.Carriable is Weapon weapon && weapon.CarriableInfo.Category != CarriableCategories.Melee)
                    {
                        slot.UpdateAmmo(FormatAmmo(weapon, player.CurrentPlayer.Inventory));
                    }
                }
            }

            if (invalidSlot)
            {
                OnSpectatingChange(player);
            }
        }

        [Event("player_inventory_clear")]
        protected void OnCarriableItemClear()
        {
            DeleteChildren(true);
        }

        [Event("player_inventory_pickup")]
        protected void OnCarriableItemPickup(Entity entity)
        {
            if (entity == null || entity is not ICarriableItem carriable)
            {
                return;
            }

            AddChild(new InventorySlot(carriable));
            SortChildren((p1, p2) =>
            {
                InventorySlot s1 = p1 as InventorySlot;
                InventorySlot s2 = p2 as InventorySlot;

                int result = s1.Carriable.CarriableInfo.Category.CompareTo(s2.Carriable.CarriableInfo.Category);

                return result != 0
                    ? result
                    : string.Compare(s1.Carriable.Info.LibraryName, s2.Carriable.Info.LibraryName, StringComparison.Ordinal);
            });

            this.Enabled(Children.Any());
        }

        [Event("player_inventory_drop")]
        protected void OnCarriableItemDrop(Entity entity)
        {
            if (entity is ICarriableItem carriable)
            {
                foreach (Panel child in Children)
                {
                    if (child is InventorySlot slot)
                    {
                        if (slot.Carriable.Info.LibraryName == carriable.Info.LibraryName)
                        {
                            child.Delete();
                        }
                    }
                }
            }

            this.Enabled(Children.Any());
        }

        [Event("player_spectating_change")]
        protected void OnSpectatingChange(Player player)
        {
            OnCarriableItemClear();

            foreach (Entity entity in player.Inventory.List)
            {
                if (entity is ICarriableItem)
                {
                    OnCarriableItemPickup(entity);
                }
            }
        }

        /// <summary>
        /// IClientInput implementation, calls during the client input build.
        /// You can both read and write to input, to affect what happens down the line.
        /// </summary>
        [Event.BuildInput]
        protected void ProcessClientInventorySelectionInput(InputBuilder input)
        {
            if (Local.Pawn is not Player player || player.IsSpectatingPlayer)
            {
                return;
            }

            if (Children == null || !Children.Any())
            {
                return;
            }

            List<Panel> childrenList = Children.ToList();
            ICarriableItem activeCarriable = player.ActiveChild as ICarriableItem;
            int keyboardIndexPressed = GetKeyboardNumberPressed(input);

            if (keyboardIndexPressed != -1)
            {
                List<ICarriableItem> weaponsOfSlotTypeSelected = new();
                int activeCarriableOfSlotTypeIndex = -1;

                for (int i = 0; i < childrenList.Count; ++i)
                {
                    if (childrenList[i] is InventorySlot slot)
                    {
                        if (Inventory.GetSlotByCategory(slot.Carriable.CarriableInfo.Category) == keyboardIndexPressed)
                        {
                            // Using the keyboard index the user pressed, find all carriables that
                            // have the same slot type as the index.
                            // Ex. "3" pressed, find all carriables with slot type "3".
                            weaponsOfSlotTypeSelected.Add(slot.Carriable);

                            if (slot.Carriable.Info.LibraryName == activeCarriable?.Info.LibraryName)
                            {
                                // If the current active carriable has the same slot type as
                                // the keyboard index the user pressed
                                activeCarriableOfSlotTypeIndex = weaponsOfSlotTypeSelected.Count - 1;
                            }
                        }
                    }
                }

                if (activeCarriable == null || activeCarriableOfSlotTypeIndex == -1)
                {
                    // The user isn't holding an active carriable, or is holding a weapon that has a different
                    // hold type than the one selected using the keyboard. We can just select the first weapon.
                    input.ActiveChild = weaponsOfSlotTypeSelected.FirstOrDefault() as Entity;
                }
                else
                {
                    // The user is holding a weapon that has the same hold type as the keyboard index the user pressed.
                    // Find the next possible weapon within the hold types.
                    input.ActiveChild = weaponsOfSlotTypeSelected[GetNextWeaponIndex(activeCarriableOfSlotTypeIndex, weaponsOfSlotTypeSelected.Count)] as Entity;
                }
            }

            int mouseWheelIndex = input.MouseWheel;

            if (mouseWheelIndex != 0)
            {
                int activeCarriableIndex = childrenList.FindIndex((p) => p is InventorySlot slot && slot.Carriable.Info.LibraryName == activeCarriable?.Info.LibraryName);
                int newSelectedIndex = NormalizeSlotIndex(-mouseWheelIndex + activeCarriableIndex, childrenList.Count - 1);

                input.ActiveChild = (childrenList[newSelectedIndex] as InventorySlot)?.Carriable as Entity;
            }
        }

        // Keyboard selection can only increment the index by 1.
        private static int GetNextWeaponIndex(int index, int count) => NormalizeSlotIndex(index + 1, count - 1);

        private static int NormalizeSlotIndex(int index, int maxIndex) => index > maxIndex ? 0 : index < 0 ? maxIndex : index;

        private int GetKeyboardNumberPressed(InputBuilder input)
        {
            for (int i = 0; i < _slotInputButtons.Length; i++)
            {
                if (input.Pressed(_slotInputButtons[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static string FormatAmmo(Weapon weapon, Inventory inventory)
        {
            if (weapon.CurrentClip.UnlimitedAmmo)
            {
                return $"{weapon.ClipAmmo} + ∞";
            }

            return $"{weapon.ClipAmmo} + {inventory.Ammo.Count(weapon.CurrentClip.AmmoName)}";
        }
    }
}
