using System;
using Sandbox;

using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    public enum WeaponType
    {
        Melee = 1,
        Pistol,
        Primary,
        Heavy,
        Special
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class WeaponAttribute : LibraryAttribute
    {
        public WeaponType WeaponType;

        public WeaponAttribute(string name) : base(name)
        {

        }
    }

    [WeaponAttribute("ttt_weapon")]
    public abstract partial class TTTWeapon : BaseWeapon, IBuyableItem
    {
        public string Name { get; private set; }
        public WeaponType WeaponType { get; private set; }
        public virtual AmmoType AmmoType => AmmoType.Pistol;
        public virtual int ClipSize => 16;
        public virtual float ReloadTime => 3.0f;
        public virtual int Bucket => 1;
        public virtual int BucketWeight => 100;
        public virtual bool UnlimitedAmmo => false;
        public virtual float ChargeAttackDuration => 2;
        public virtual bool HasFlashlight => false;
        public virtual bool HasLaserDot => false;
        public virtual int BaseDamage => 10;
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
        // TODO add player role to weapon to access in UI WeaponSelection.cs.
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

        public TTTWeapon() : base()
        {
            WeaponAttribute weaponAttribute = Library.GetAttribute(GetType()) as WeaponAttribute;

            Name = weaponAttribute.Name;
            WeaponType = weaponAttribute.WeaponType;
        }

        public virtual int GetPrice() => 200;

        public virtual bool IsBuyable(TTTPlayer player)
        {
            return !(player.Inventory as Inventory).IsCarryingType(GetType());
        }

        public string GetName() => Name;

        public void Equip(TTTPlayer player)
        {
            player.Inventory.Add(this);
        }

        public int AvailableAmmo()
        {
            if (Owner is not TTTPlayer owner)
            {
                return 0;
            }

            return owner.AmmoCount(AmmoType);
        }

        public override void ActiveStart(Entity owner)
        {
            base.ActiveStart(owner);

            TimeSinceDeployed = 0;
        }

        public override void Spawn()
        {
            base.Spawn();

            AmmoClip = ClipSize;

            SetModel("weapons/rust_pistol/rust_pistol.vmdl");

            PickupTrigger = new PickupTrigger();
            PickupTrigger.Parent = this;
            PickupTrigger.Position = Position;
        }

        public override void Reload()
        {
            if (WeaponType == WeaponType.Melee || IsReloading)
            {
                return;
            }

            if (AmmoClip >= ClipSize)
            {
                return;
            }

            TimeSinceReload = 0;

            if (Owner is TTTPlayer player)
            {
                if (!UnlimitedAmmo && player.AmmoCount(AmmoType) <= 0)
                {
                    return;
                }
            }

            IsReloading = true;

            (Owner as AnimEntity).SetAnimBool("b_reload", true);

            DoClientReload();
        }

        public override void Simulate(Client owner)
        {
            if (owner.Pawn is TTTPlayer player)
            {
                if (owner.Pawn.LifeState == LifeState.Alive)
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
            if (ChargeAttackEndTime > 0f && Time.Now < ChargeAttackEndTime)
            {
                return false;
            }

            return base.CanPrimaryAttack();
        }

        public override bool CanSecondaryAttack()
        {
            if (ChargeAttackEndTime > 0f && Time.Now < ChargeAttackEndTime)
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

            if (!(Owner is TTTPlayer player))
            {
                return;
            }

            if (!UnlimitedAmmo)
            {
                int ammo = player.TakeAmmo(AmmoType, ClipSize - AmmoClip);

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

            if (WeaponType != WeaponType.Melee)
            {
                Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
            }

            if (IsLocalPawn)
            {
                new Sandbox.ScreenShake.Perlin();
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
                tr.Surface.DoBulletImpact(tr);

                if (!IsServer || !tr.Entity.IsValid())
                {
                    continue;
                }

                using (Prediction.Off())
                {
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

            // TODO: Give users a way to change their crosshair.
            CrosshairPanel = new Crosshair().SetupCrosshair(new Crosshair.Properties(true,
                false,
                false,
                10,
                2,
                0,
                0,
                Color.Green));
            CrosshairPanel.Parent = Local.Hud;
            CrosshairPanel.AddClass(ClassInfo.Name);
        }

        public bool IsUsable()
        {
            if (WeaponType == WeaponType.Melee || ClipSize == 0 || AmmoClip > 0)
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
    }
}
