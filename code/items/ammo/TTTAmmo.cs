using System;
using Sandbox;
using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_ammo"), Hammer.Skip]
    public abstract partial class TTTAmmo : Prop
    {
        /// <summary>
        /// String definition of ammo type, should match TTTWeapon.AmmoType
        /// </summary>
        public virtual string AmmoType { get; set; }
        /// <summary>
        /// Amount of Ammo within Entity.
        /// </summary>
        public virtual int AmmoAmount { get; set; }

        /// <summary>
        /// Maximum amount of ammo player can carry in their inventory.
        /// </summary>
        public virtual int MaxAmmo { get; set; }

        [Net]
        private int CurrentAmmo { get; set; }
        private int AmmoEntMax { get; set; }

        public virtual string ModelPath => "models/ammo_buckshot.vmdl";

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
            RemoveCollisionLayer(CollisionLayer.Player);

            AmmoEntMax = AmmoAmount;
            CurrentAmmo = AmmoAmount;
        }

        public override void Touch(Entity other)
        {
            base.Touch(other);
            if (IsServer)
            {
                if(other is TTTPlayer player)
                {
                    var ammoType = AmmoType.ToLower();
                    var playerInv = player.Inventory as Inventory;

                    if (playerInv.GetAmmoTypes().Contains(ammoType))
                    {
                        var playerAmount = playerInv.Ammo.Count(ammoType);

                        if (MaxAmmo >= (playerAmount + Math.Ceiling(CurrentAmmo * 0.25)))
                        {
                            var amountGiven = Math.Min(CurrentAmmo, MaxAmmo - playerAmount);
                            playerInv.Ammo.Give(ammoType, amountGiven);
                            CurrentAmmo -= amountGiven;

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
            //Excerpt from Sandbox.BasePhysics | Removes the ability to remove entity health and destroy, but will start calculate appropriate physics for damage.
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
