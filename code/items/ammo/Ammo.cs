using System;

using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Hammer.Skip]
    public abstract partial class Ammo : Prop, IEntityHint, IPickupable
    {
        /// <summary>
        /// The library name of the ammo.
        /// </summary>
        public string LibraryName { get; set; }

        /// <summary>
        /// The name of the connected weapon (for translation)
        /// </summary>
        public string WeaponName { get; set; }

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

        public PickupTrigger PickupTrigger { get; set; }

        public Entity LastDropOwner { get; set; }
        public TimeSince SinceLastDrop { get; set; } = 0f;

        /// <summary>
        /// Fired when a player picks up any amount of ammo from the entity.
        /// </summary>
        protected Output OnPickup { get; set; }

        public override string ModelPath => "models/ammo/ammo_buckshot.vmdl";

        public Ammo() : base()
        {
            LibraryName = Utils.GetLibraryName(GetType());
            WeaponName ??= LibraryName.Replace("ttt_ammo_", "ttt_weapon_");
        }

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
            CollisionGroup = CollisionGroup.Weapon;
            SetInteractsAs(CollisionLayer.Debris);

            AmmoEntMax = Amount;
            CurrentAmmo = Amount;

            Tags.Add(IItem.ITEM_TAG);

            PickupTrigger = new();
            PickupTrigger.Parent = this;
            PickupTrigger.Position = Position;
            PickupTrigger.Rotation = Rotation;
        }

        public void SetCurrentAmmo(int ammo)
        {
            CurrentAmmo = ammo;
        }

        public override void TakeDamage(DamageInfo info)
        {
            PhysicsBody body = info.Body;

            if (!body.IsValid())
            {
                body = PhysicsBody;
            }

            if (body.IsValid() && !info.Flags.HasFlag(DamageFlags.PhysicsImpact))
            {
                body.ApplyImpulseAt(info.Position, info.Force * 100);
            }
        }

        public float HintDistance => 80f;

        public TranslationData TextOnTick => new("USE.PICKUP", Input.GetKeyWithBinding("+iv_use").ToUpper(), new TranslationData("ITEM.WEAPON.AMMO.WEAPON", new TranslationData(IItem.GetTranslationKey(WeaponName, "NAME"))));

        public bool CanHint(TTTPlayer client)
        {
            return true;
        }

        public EntityHintPanel DisplayHint(TTTPlayer client)
        {
            return new Hint(TextOnTick);
        }

        public virtual void PickupStartTouch(Entity other)
        {
            if (other != LastDropOwner && other is TTTPlayer player)
            {
                Pickup(player);
            }
        }

        public virtual void PickupEndTouch(Entity other)
        {
            if (other is TTTPlayer && LastDropOwner == other)
            {
                LastDropOwner = null;
            }
        }

        public void Tick(TTTPlayer player)
        {
            if (IsClient || player.LifeState != LifeState.Alive)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (!Input.Pressed(InputButton.Use))
                {
                    return;
                }

                Pickup(player);
            }
        }

        public void Pickup(TTTPlayer player)
        {
            string ammoType = LibraryName.ToLower();
            Inventory inventory = player.Inventory;

            if (!inventory.GetAmmoNames().Contains(ammoType))
            {
                return;
            }

            int playerAmount = inventory.Ammo.Count(ammoType);

            if (Max < playerAmount + Math.Ceiling(CurrentAmmo * 0.25))
            {
                return;
            }

            int amountGiven = Math.Min(CurrentAmmo, Max - playerAmount);
            inventory.Ammo.Give(ammoType, amountGiven);
            CurrentAmmo -= amountGiven;

            _ = OnPickup.Fire(player);

            if (CurrentAmmo <= 0 || Math.Ceiling(AmmoEntMax * 0.25) > CurrentAmmo)
            {
                Delete();
            }
        }

        public override void Simulate(Client owner)
        {
            if (SinceLastDrop > 0.5f)
            {
                LastDropOwner = null;
            }

            base.Simulate(owner);
        }
    }
}
