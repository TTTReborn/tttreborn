using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_ammo")]
    public abstract partial class TTTAmmo : Prop
    {
        /// <summary>
        /// String definition of ammo type, should match TTTWeapon.AmmoType
        /// </summary>
        public virtual string Type { get; set; }
        /// <summary>
        /// Amount of Ammo within Entity.
        /// </summary>
        public virtual int Amount { get; set; }

        /// <summary>
        /// Maximum amount of ammo player can carry in their inventory.
        /// </summary>
        public virtual int Max { get; set; }

        [Net]
        private int CurrentAmmo { get; set; }
        private int AmmoEntMax { get; set; }

        /// <summary>
        /// Fired when a player picks up any amount of ammo from the entity.
        /// </summary>
        protected Output OnPickup { get; set; }

        public virtual string ModelPath => "models/ammo/ammo_buckshot.vmdl";

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);

            AmmoEntMax = Amount;
            CurrentAmmo = Amount;
        }

        public override void Touch(Entity other)
        {
            base.Touch(other);
            if (IsServer)
            {
                if (other is TTTPlayer player)
                {
                    var ammoType = Type.ToLower();
                    var playerInv = player.Inventory as Inventory;

                    if (playerInv.GetAmmoTypes().Contains(ammoType)) //Possible optimization point by caching weapons that can our ammo, rather than checking if player has our ammo.
                    {
                        var playerAmount = playerInv.Ammo.Count(ammoType);

                        if (Max >= (playerAmount + Math.Ceiling(CurrentAmmo * 0.25)))
                        {
                            var amountGiven = Math.Min(CurrentAmmo, Max - playerAmount);
                            playerInv.Ammo.Give(ammoType, amountGiven);
                            CurrentAmmo -= amountGiven;
                            OnPickup.Fire(other);

                            if (CurrentAmmo <= 0 || Math.Ceiling(AmmoEntMax * 0.25) > CurrentAmmo)
                            {
                                Delete();
                            }
                        }
                    }
                }
            }
        }

        public void SetCurrentAmmo(int ammo)
        {
            CurrentAmmo = ammo;
        }

        public override void TakeDamage(DamageInfo info)
        {
            //Excerpt from Sandbox.BasePhysics | Removes the ability to remove entity health and destroy, but will still calculate appropriate physics for damage.
            //Unsure how Prop.Invulnerable works in comparison to this. Might be a better solution.
            var body = info.Body;
            if (!body.IsValid())
                body = PhysicsBody;

            if (body.IsValid() && !info.Flags.HasFlag(DamageFlags.PhysicsImpact))
            {
                body.ApplyImpulseAt(info.Position, info.Force * 100);
            }
            return;
        }
    }
}
