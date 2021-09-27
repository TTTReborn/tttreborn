using System;

using Sandbox;
using Sandbox.ScreenShake;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    // DO NOT DELETE
    // This should be added by sbox soonTM (so we gonna be able to fetch data without the need initializing and spawning such a weapon)
    //
    // [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    // public class WeaponAttribute : LibraryAttribute
    // {
    //     public WeaponType WeaponType;

    //     public WeaponAttribute(string name) : base(name)
    //     {

    //     }
    // }

    [Library("ttt_weapon")]
    public abstract partial class TTTWeapon : BaseWeapon, ICarriableItem
    {
        public virtual SlotType SlotType => SlotType.Primary;
        public virtual string AmmoType => "pistol";
        public virtual Type AmmoEntity => null;
        public virtual int ClipSize => 16;
        public virtual float ReloadTime => 3.0f;
        public virtual float DeployTime => 0.6f;
        public virtual bool UnlimitedAmmo => false;
        public virtual float ChargeAttackDuration => 2;
        public virtual int BaseDamage => 10;
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
        // TODO add player role to weapon to access in UI InventorySelection.cs.
        // E.G. this weapon is bought in traitor shop: Role => "Traitor";
        // This weapon is a normal weapon: Role => "None"

        [Net, Predicted]
        public int AmmoClip { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceReload { get; set; }

        [Net, Predicted]
        public bool IsReloading { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceDeployed { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceChargeAttack { get; set; }

        public float ChargeAttackEndTime;

        public PickupTrigger PickupTrigger { get; protected set; }

        public string ClassName { get; }

        private const int AmmoDropPositionOffset = 50;
        private const int AmmoDropVelocity = 500;

        public TTTWeapon() : base()
        {
            ClassName = Library.GetAttribute(GetType()).Name;
            Tags.Add(IItem.ITEM_TAG);
        }

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

        public int AvailableAmmo()
        {
            if (Owner is not TTTPlayer owner)
            {
                return 0;
            }

            return owner.Inventory.Ammo.Count(AmmoType);
        }

        public override void ActiveStart(Entity owner)
        {
            base.ActiveStart(owner);

            TimeSinceDeployed = 0;

            IsReloading = false;
        }

        public override void Spawn()
        {
            base.Spawn();

            AmmoClip = ClipSize;

            SetModel("weapons/rust_pistol/rust_pistol.vmdl");

            PickupTrigger = new();
            PickupTrigger.Parent = this;
            PickupTrigger.Position = Position;
        }

        public override void Reload()
        {
            if (SlotType == SlotType.Melee || IsReloading || AmmoClip >= ClipSize)
            {
                return;
            }

            TimeSinceReload = 0;

            if (Owner is TTTPlayer player && !UnlimitedAmmo && player.Inventory.Ammo.Count(AmmoType) <= 0)
            {
                return;
            }

            IsReloading = true;

            (Owner as AnimEntity).SetAnimBool("b_reload", true);

            DoClientReload();
        }

        public override void Simulate(Client owner)
        {
            if (TimeSinceDeployed < DeployTime)
            {
                return;
            }

            if (owner.Pawn is TTTPlayer player)
            {
                if (player.LifeState == LifeState.Alive)
                {
                    if (ChargeAttackEndTime > 0f && Time.Now >= ChargeAttackEndTime)
                    {
                        OnChargeAttackFinish();

                        ChargeAttackEndTime = 0f;
                    }
                }
                else
                {
                    ChargeAttackEndTime = 0f;
                }
            }

            if (Input.Released(InputButton.View) && AmmoClip > 0 && !UnlimitedAmmo)
            {

                if (IsServer && AmmoEntity != null)
                {
                    TTTAmmo ammoBox = Globals.Utils.GetObjectByType<TTTAmmo>(AmmoEntity);

                    ammoBox.Position = Owner.EyePos + Owner.EyeRot.Forward * AmmoDropPositionOffset;
                    ammoBox.Rotation = Owner.EyeRot;
                    ammoBox.Velocity = Owner.EyeRot.Forward * AmmoDropVelocity;
                    ammoBox.SetCurrentAmmo(AmmoClip);
                }

                TakeAmmo(AmmoClip);
            }

            if (!IsReloading)
            {
                base.Simulate(owner);
            }
            else if (TimeSinceReload > ReloadTime)
            {
                OnReloadFinish();
            }
        }

        public override bool CanPrimaryAttack()
        {
            if (ChargeAttackEndTime > 0f && Time.Now < ChargeAttackEndTime || TimeSinceDeployed <= DeployTime)
            {
                return false;
            }

            return base.CanPrimaryAttack();
        }

        public override bool CanSecondaryAttack()
        {
            if (ChargeAttackEndTime > 0f && Time.Now < ChargeAttackEndTime || TimeSinceDeployed <= DeployTime)
            {
                return false;
            }

            return base.CanSecondaryAttack();
        }

        public virtual void StartChargeAttack()
        {
            ChargeAttackEndTime = Time.Now + ChargeAttackDuration;
        }

        public virtual void OnChargeAttackFinish()
        {

        }

        public virtual void OnReloadFinish()
        {
            IsReloading = false;

            if (Owner is not TTTPlayer player)
            {
                return;
            }

            if (!UnlimitedAmmo)
            {
                int ammo = player.Inventory.Ammo.Take(AmmoType, ClipSize - AmmoClip);

                if (ammo == 0)
                {
                    return;
                }

                AmmoClip += ammo;
            }
            else
            {
                AmmoClip = ClipSize;
            }
        }

        [ClientRpc]
        public virtual void DoClientReload()
        {
            ViewModelEntity?.SetAnimBool("reload", true);
        }

        public override void AttackPrimary()
        {
            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

            ShootEffects();

            ShootBullet(0.05f, 1.5f, BaseDamage, 3.0f);
        }

        [ClientRpc]
        protected virtual void ShootEffects()
        {
            Host.AssertClient();

            if (SlotType != SlotType.Melee)
            {
                Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
            }

            if (IsLocalPawn)
            {
                new Perlin();
            }

            ViewModelEntity?.SetAnimBool("fire", true);
        }

        public virtual void ShootBullet(float spread, float force, float damage, float bulletSize)
        {
            Vector3 forward = Owner.EyeRot.Forward;
            forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
            forward = forward.Normal;

            foreach (TraceResult tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * 5000, bulletSize))
            {
                if (!IsServer || !tr.Entity.IsValid())
                {
                    continue;
                }

                using (Prediction.Off())
                {
                    tr.Surface.DoBulletImpact(tr);

                    DamageInfo damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Entity.TakeDamage(damageInfo);
                }
            }
        }

        public bool TakeAmmo(int amount)
        {
            if (AmmoClip < amount)
            {
                return false;
            }

            AmmoClip -= amount;

            return true;
        }

        public override void CreateViewModel()
        {
            Host.AssertClient();

            if (string.IsNullOrEmpty(ViewModelPath))
            {
                return;
            }

            ViewModelEntity = new ViewModel
            {
                Position = Position,
                Owner = Owner,
                EnableViewmodelRendering = true
            };

            ViewModelEntity.SetModel(ViewModelPath);
        }

        public override void CreateHudElements()
        {
            if (Local.Hud == null)
            {
                return;
            }
        }

        public bool IsUsable()
        {
            if (SlotType == SlotType.Melee || ClipSize == 0 || AmmoClip > 0)
            {
                return true;
            }

            return AvailableAmmo() > 0;
        }

        public override void OnCarryStart(Entity carrier)
        {
            base.OnCarryStart(carrier);

            if (PickupTrigger.IsValid())
            {
                PickupTrigger.EnableTouch = false;
            }
        }

        public override void OnCarryDrop(Entity dropper)
        {
            base.OnCarryDrop(dropper);

            if (PickupTrigger.IsValid())
            {
                PickupTrigger.EnableTouch = true;
            }
        }

        public virtual bool CanDrop() => true;
    }
}
