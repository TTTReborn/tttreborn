using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.ScreenShake;

using TTTReborn.Globalization;
using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WeaponAttribute : CarriableAttribute
    {
        public string AmmoName { get; private set; }
        public Type AmmoType { get; set; }

        public WeaponAttribute(CarriableCategories category) : base(category)
        {
            if (AmmoType == null)
            {
                switch (category)
                {
                    case CarriableCategories.Pistol:
                        AmmoType = typeof(PistolAmmo);

                        break;
                    case CarriableCategories.SMG:
                        AmmoType = typeof(SMGAmmo);

                        break;
                    case CarriableCategories.Shotgun:
                        AmmoType = typeof(ShotgunAmmo);

                        break;
                    case CarriableCategories.Sniper:
                        AmmoType = typeof(SniperAmmo);

                        break;
                }
            }

            if (AmmoType != null && Library.GetAttribute(AmmoType) != null)
            {
                AmmoName = Utils.GetLibraryName(AmmoType);
            }
            else
            {
                AmmoType = null;
            }
        }
    }

    [Hammer.Skip]
    public abstract partial class TTTWeapon : BaseWeapon, ICarriableItem, IEntityHint
    {
        public string LibraryName { get; }
        public CarriableCategories Category { get; } = CarriableCategories.Pistol;
        public Type AmmoType { get; }
        public string AmmoName { get; }
        public virtual int ClipSize => 16;
        public virtual float ReloadTime => 3.0f;
        public virtual float DeployTime => 0.6f;
        public virtual bool UnlimitedAmmo => false;
        public virtual float ChargeAttackDuration => 2;
        public virtual int BaseDamage => 10;
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
        public virtual string ModelPath => "weapons/rust_pistol/rust_pistol.vmdl";

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

        private const int AMMO_DROP_POSITION_OFFSET = 50;
        private const int AMMO_DROP_VELOCITY = 500;

        public TTTWeapon() : base()
        {
            LibraryName = Utils.GetLibraryName(GetType());

            foreach (object obj in GetType().GetCustomAttributes(false))
            {
                if (obj is WeaponAttribute weaponAttribute)
                {
                    Category = weaponAttribute.Category;
                    AmmoType = weaponAttribute.AmmoType;
                    AmmoName = weaponAttribute.AmmoName;
                }
            }

            EnableShadowInFirstPerson = false;

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
            if (Owner is not TTTPlayer owner || AmmoName == null)
            {
                return 0;
            }

            return owner.Inventory.Ammo.Count(AmmoName);
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

            SetModel(ModelPath);

            PickupTrigger = new();
            PickupTrigger.Parent = this;
            PickupTrigger.Position = Position;
        }

        public override void Reload()
        {
            if (Category == CarriableCategories.Melee || IsReloading || AmmoClip >= ClipSize)
            {
                return;
            }

            TimeSinceReload = 0;

            if (Owner is TTTPlayer player && !UnlimitedAmmo && (AmmoName == null || player.Inventory.Ammo.Count(AmmoName) <= 0))
            {
                return;
            }

            IsReloading = true;

            (Owner as AnimEntity).SetAnimParameter("b_reload", true);

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

            if (Input.Pressed(InputButton.Drop) && Input.Down(InputButton.Run) && AmmoClip > 0 && !UnlimitedAmmo)
            {
                if (IsServer && AmmoType != null)
                {
                    TTTAmmo ammoBox = Utils.GetObjectByType<TTTAmmo>(AmmoType);

                    ammoBox.Position = Owner.EyePosition + Owner.EyeRotation.Forward * AMMO_DROP_POSITION_OFFSET;
                    ammoBox.Rotation = Owner.EyeRotation;
                    ammoBox.Velocity = Owner.EyeRotation.Forward * AMMO_DROP_VELOCITY;
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

            if (Owner is not TTTPlayer player || AmmoName == null)
            {
                return;
            }

            if (!UnlimitedAmmo)
            {
                int ammo = player.Inventory.Ammo.Take(AmmoName, ClipSize - AmmoClip);

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
            ViewModelEntity?.SetAnimParameter("reload", true);
        }

        public override void AttackPrimary()
        {
            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

            if (IsClient)
            {
                ShootEffects();
            }

            ShootBullet(0.05f, 1.5f, BaseDamage, 3.0f);
        }

        protected virtual void ShootEffects()
        {
            Host.AssertClient();

            if (Category != CarriableCategories.Melee)
            {
                Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
            }

            if (IsLocalPawn)
            {
                using (Prediction.Off())
                {
                    _ = new Perlin();
                }
            }

            ViewModelEntity?.SetAnimParameter("fire", true);
        }

        public virtual void ShootBullet(float spread, float force, float damage, float bulletSize)
        {
            Vector3 forward = Owner.EyeRotation.Forward;
            forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
            forward = forward.Normal;

            foreach (TraceResult tr in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * 5000, bulletSize))
            {
                if (!IsServer || !tr.Entity.IsValid())
                {
                    continue;
                }

                using (Prediction.Off())
                {
                    tr.Surface.DoBulletImpact(tr);

                    DamageInfo damageInfo = DamageInfo.FromBullet(tr.EndPosition, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Entity.TakeDamage(damageInfo);
                }
            }
        }

        public override IEnumerable<TraceResult> TraceBullet(Vector3 start, Vector3 end, float radius = 2.0f)
        {
            using (LagCompensation())
            {
                bool InWater = Sandbox.Internal.GlobalGameNamespace.Map.Physics.IsPointWater(start);

                TraceResult tr = Trace.Ray(start, end)
                        .UseHitboxes()
                        .HitLayer(CollisionLayer.Water, !InWater)
                        .HitLayer(CollisionLayer.Debris)
                        .Ignore(Owner)
                        .Ignore(this)
                        .Size(radius)
                        .Run();

                yield return tr;
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
            if (Category == CarriableCategories.Melee || ClipSize == 0 || AmmoClip > 0)
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
                    if (player.Inventory.Active is ICarriableItem carriable && carriable.Category == Category)
                    {
                        player.Inventory.DropActive();
                    }

                    player.Inventory.TryAdd(this, deleteIfFails: false, makeActive: true);
                }
            }
        }
    }
}
