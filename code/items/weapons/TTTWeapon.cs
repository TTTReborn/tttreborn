using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.ScreenShake;

using TTTReborn.Globalization;
using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WeaponAttribute : CarriableAttribute
    {

        public WeaponAttribute() : base()
        {

        }
    }

    [Hammer.Skip]
    public abstract partial class TTTWeapon : SWB_Base.WeaponBase, ICarriableItem, IEntityHint
    {
        public string LibraryName { get; }
        public SlotType SlotType { get; } = SlotType.Secondary;

        public TTTWeapon() : base()
        {
            LibraryName = Utils.GetLibraryName(GetType());

            foreach (object obj in GetType().GetCustomAttributes(false))
            {
                if (obj is WeaponAttribute weaponAttribute)
                {
                    SlotType = weaponAttribute.SlotType;
                }
            }

            EnableShadowInFirstPerson = false;

            Tags.Add(IItem.ITEM_TAG);
        }

        public new bool CanDrop() => true;

        public void Equip(TTTPlayer player)
        {
            OnEquip();
        }

        public virtual void OnEquip()
        {

        }

        public void Remove()
        {
            OnRemove();
        }

        public virtual void OnRemove()
        {

        }

        public float HintDistance => 80f;

        public TranslationData TextOnTick => new("GENERIC_PICKUP", Input.GetKeyWithBinding("+iv_use").ToUpper(), new TranslationData(LibraryName.ToUpper()));

        public bool CanHint(TTTPlayer client)
        {
            return true;
        }

        public EntityHintPanel DisplayHint(TTTPlayer client)
        {
            return new Hint(TextOnTick);
        }

        public void Tick(TTTPlayer player)
        {
            if (IsClient)
            {
                return;
            }

            if (player.LifeState != LifeState.Alive)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Use))
                {
                    if (player.Inventory.Active is ICarriableItem carriable && carriable.SlotType == SlotType)
                    {
                        player.Inventory.DropActive();
                    }

                    player.Inventory.TryAdd(this, deleteIfFails: false, makeActive: true);
                }
            }
        }
    }
}
