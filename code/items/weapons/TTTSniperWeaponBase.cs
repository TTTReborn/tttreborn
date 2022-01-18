using System;

using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Hammer.Skip]
    public abstract partial class TTTSniperWeaponBase : SWB_Base.WeaponBaseSniper, ICarriableItem, IEntityHint
    {
        public string LibraryName { get; }
        public SlotType SlotType { get; } = SlotType.Secondary;
        public virtual Type AmmoEntity => null;
        private const int AMMO_DROP_POSITION_OFFSET = 50;
        private const int AMMO_DROP_VELOCITY = 500;

        public TTTSniperWeaponBase() : base()
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

        public override void Simulate(Client owner)
        {
            if (Input.Pressed(InputButton.Drop) && Input.Down(InputButton.Run) && Primary.Ammo > 0 && Primary.InfiniteAmmo == SWB_Base.InfiniteAmmoType.normal)
            {
                if (IsServer && AmmoEntity != null)
                {
                    TTTAmmo ammoBox = Utils.GetObjectByType<TTTAmmo>(AmmoEntity);

                    ammoBox.Position = Owner.EyePos + Owner.EyeRot.Forward * AMMO_DROP_POSITION_OFFSET;
                    ammoBox.Rotation = Owner.EyeRot;
                    ammoBox.Velocity = Owner.EyeRot.Forward * AMMO_DROP_VELOCITY;
                    ammoBox.SetCurrentAmmo(Primary.Ammo);
                }

                Primary.Ammo -= Primary.Ammo;
            }

            base.Simulate(owner);
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
