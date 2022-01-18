using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WeaponAttribute : CarriableAttribute
    {

        public WeaponAttribute() : base()
        {

        }
    }

    public static partial class TTTWeaponBaseGeneric
    {
        private const int AMMO_DROP_POSITION_OFFSET = 50;
        private const int AMMO_DROP_VELOCITY = 500;

        public static void Simulate(Client owner, Type AmmoEntity, SWB_Base.ClipInfo clip)
        {
            if (Input.Pressed(InputButton.Drop) && Input.Down(InputButton.Run) && clip.Ammo > 0 && clip.InfiniteAmmo == SWB_Base.InfiniteAmmoType.normal)
            {
                if (Host.IsServer)
                {
                    TTTAmmo ammoBox = Utils.GetObjectByType<TTTAmmo>(AmmoEntity);

                    ammoBox.Position = owner.Pawn.EyePos + owner.Pawn.EyeRot.Forward * AMMO_DROP_POSITION_OFFSET;
                    ammoBox.Rotation = owner.Pawn.EyeRot;
                    ammoBox.Velocity = owner.Pawn.EyeRot.Forward * AMMO_DROP_VELOCITY;
                    ammoBox.SetCurrentAmmo(clip.Ammo);
                }

                clip.Ammo -= clip.Ammo;
            }
        }

        public static void Tick(TTTPlayer player, ICarriableItem item)
        {
            if (Host.IsClient)
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
                    if (player.Inventory.Active is ICarriableItem carriable && carriable.SlotType == item.SlotType)
                    {
                        player.Inventory.DropActive();
                    }

                    player.Inventory.TryAdd(item, deleteIfFails: false, makeActive: true);
                }
            }
        }
    }
}
