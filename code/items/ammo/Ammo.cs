using System;

using Sandbox;

using TTTReborn.Entities;
using TTTReborn.Globalization;
using TTTReborn.UI;

namespace TTTReborn.Items
{
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
        public TimeSince TimeSinceLastDrop { get; set; } = 0f;

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
            Tags.Add("debris");

            AmmoEntMax = Amount;
            CurrentAmmo = Amount;

            Tags.Add(IItem.ITEM_TAG);

            PickupTrigger = new()
            {
                Parent = this,
                Position = Position,
                Rotation = Rotation
            };
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

            if (body.IsValid() && !info.HasTag("physicsimpact"))
            {
                body.ApplyImpulseAt(info.Position, info.Force * 100);
            }
        }

        public float HintDistance => 80f;

        public TranslationData[] TextOnTick => new TranslationData[] { new("WEAPON.USE", new TranslationData("WEAPON.AMMO.WEAPON", new TranslationData(Utils.GetTranslationKey(WeaponName, "NAME")))) };

        public bool CanHint(Player client) => true;

        public EntityHintPanel DisplayHint(Player client) => new GlyphHint(new GlyphHintData[] { new(TextOnTick[0], InputButton.Use) });

        public virtual void PickupStartTouch(Entity other)
        {
            if ((other != LastDropOwner || TimeSinceLastDrop > 0.25f) && other is Player player)
            {
                LastDropOwner = null;

                Pickup(player);
            }
        }

        public virtual void PickupEndTouch(Entity other)
        {
            if (other is Player && LastDropOwner == other)
            {
                LastDropOwner = null;
            }
        }

        public void HintTick(Player player)
        {
            if (Game.IsClient || player.LifeState != LifeState.Alive)
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

        public void Pickup(Player player)
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
    }
}
