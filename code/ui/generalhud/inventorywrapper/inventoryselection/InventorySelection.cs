using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Player;

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

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            foreach (Entity entity in player.CurrentPlayer.Inventory.List)
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

            ICarriableItem activeItem = player.CurrentPlayer.ActiveChild as ICarriableItem;

            bool invalidSlot = false;

            foreach (Sandbox.UI.Panel child in Children)
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

                    slot.SetClass("active", slot.Carriable.ClassName == activeItem?.ClassName);
                    slot.SetClass("opacity-heavy", slot.Carriable.ClassName == activeItem?.ClassName);

                    if (slot.Carriable is TTTWeapon weapon && weapon.SlotType != SlotType.Melee)
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

        [Event("tttreborn.player.inventory.clear")]
        private void OnCarriableItemClear()
        {
            DeleteChildren(true);
        }

        [Event("tttreborn.player.carriableitem.pickup")]
        private void OnCarriableItemPickup(ICarriableItem carriable)
        {
            if (carriable == null)
            {
                return;
            }

            AddChild(new InventorySlot(this, carriable));
            SortChildren((p1, p2) =>
            {
                InventorySlot s1 = p1 as InventorySlot;
                InventorySlot s2 = p2 as InventorySlot;

                int result = s1.Carriable.SlotType.CompareTo(s2.Carriable.SlotType);
                return result != 0
                    ? result
                    : String.Compare(s1.Carriable.ClassName, s2.Carriable.ClassName, StringComparison.Ordinal);
            });

            Enabled = Children.Any();
        }

        [Event("tttreborn.player.carriableitem.drop")]
        private void OnCarriableItemDrop(ICarriableItem carriable)
        {
            foreach (Sandbox.UI.Panel child in Children)
            {
                if (child is InventorySlot slot)
                {
                    if (slot.Carriable.ClassName == carriable.ClassName)
                    {
                        child.Delete();
                    }
                }
            }

            Enabled = Children.Any();
        }

        [Event("tttreborn.player.spectating.change")]
        private void OnSpectatingChange(TTTPlayer player)
        {
            OnCarriableItemClear();

            foreach (Entity entity in player.Inventory.List)
            {
                if (entity is ICarriableItem carriableItem)
                {
                    OnCarriableItemPickup(carriableItem);
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
            if (Local.Pawn is not TTTPlayer player || player.IsSpectatingPlayer)
            {
                return;
            }

            if (Children == null || !Children.Any())
            {
                return;
            }

            List<Sandbox.UI.Panel> childrenList = Children.ToList();

            ICarriableItem activeCarriable = Local.Pawn.ActiveChild as ICarriableItem;

            int keyboardIndexPressed = GetKeyboardNumberPressed(input);
            if (keyboardIndexPressed != -1)
            {
                List<ICarriableItem> weaponsOfSlotTypeSelected = new();
                int activeCarriableOfSlotTypeIndex = -1;

                for (int i = 0; i < childrenList.Count; ++i)
                {
                    if (childrenList[i] is InventorySlot slot)
                    {
                        if ((int) slot.Carriable.SlotType == keyboardIndexPressed)
                        {
                            // Using the keyboard index the user pressed, find all carriables that
                            // have the same slot type as the index.
                            // Ex. "3" pressed, find all carriables with slot type "3".
                            weaponsOfSlotTypeSelected.Add(slot.Carriable);

                            if (slot.Carriable.ClassName == activeCarriable?.ClassName)
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
                int activeCarriableIndex = childrenList.FindIndex((p) =>
                    p is InventorySlot slot && slot.Carriable.ClassName == activeCarriable?.ClassName);

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
            for (int i = 0; i < _slotInputButtons.Length; i++)
            {
                if (input.Pressed(_slotInputButtons[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private static string FormatAmmo(TTTWeapon weapon, Inventory inventory)
        {
            if (weapon.UnlimitedAmmo)
            {
                return $"{weapon.AmmoClip} + ∞";
            }

            return $"{weapon.AmmoClip} + {(inventory.Ammo.Count(weapon.AmmoType))}";
        }

        private class InventorySlot : Panel
        {
            public ICarriableItem Carriable { get; init; }
            public Label SlotLabel;
            private readonly Label _ammoLabel;
            private Label _carriableLabel;

            public InventorySlot(Sandbox.UI.Panel parent, ICarriableItem carriable) : base(parent)
            {
                Parent = parent;
                Carriable = carriable;

                AddClass("background-color-primary");

                SlotLabel = Add.Label(((int) carriable.SlotType).ToString());
                SlotLabel.AddClass("slot-label");

                _carriableLabel = Add.Label(carriable.ClassName);

                _ammoLabel = Add.Label();

                if (Local.Pawn is TTTPlayer player)
                {
                    if (carriable is TTTWeapon weapon && carriable.SlotType != SlotType.Melee)
                    {
                        _ammoLabel.Text = FormatAmmo(weapon, player.Inventory);
                        _ammoLabel.AddClass("ammo-label");
                    }
                }
            }

            public override void Tick()
            {
                base.Tick();

                if (Local.Pawn is TTTPlayer player)
                {
                    SlotLabel.Style.BackgroundColor = player.Team.Color;
                }
            }

            public void UpdateAmmo(string ammoText)
            {
                _ammoLabel.Text = ammoText;
            }
        }
    }
}
