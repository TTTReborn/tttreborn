using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.ScreenShake;

using TTTReborn.Entities;
using TTTReborn.Globalization;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WeaponAttribute : CarriableAttribute
    {
        public string PrimaryAmmoName { get; private set; }
        public string SecondaryAmmoName { get; private set; }

        public Type PrimaryAmmoType { get; set; }
        public Type SecondaryAmmoType { get; set; }

        public WeaponAttribute(CarriableCategories category) : base(category)
        {
            PrimaryAmmoType ??= GetAmmoType(category);

            if (PrimaryAmmoType != null && Library.GetAttribute(PrimaryAmmoType) != null)
            {
                PrimaryAmmoName = Utils.GetLibraryName(PrimaryAmmoType);
            }

            SecondaryAmmoType ??= GetAmmoType(category);

            if (SecondaryAmmoType != null && Library.GetAttribute(SecondaryAmmoType) != null)
            {
                SecondaryAmmoName = Utils.GetLibraryName(SecondaryAmmoType);
            }
        }

        private static Type GetAmmoType(CarriableCategories category)
        {
            return category switch
            {
                CarriableCategories.Pistol => typeof(PistolAmmo),
                CarriableCategories.SMG => typeof(SMGAmmo),
                CarriableCategories.Shotgun => typeof(ShotgunAmmo),
                CarriableCategories.Sniper => typeof(SniperAmmo),
                CarriableCategories.OffensiveEquipment => typeof(OffensiveEquipmentAmmo),
                _ => null
            };
        }
    }

    [Precached("particles/pistol_muzzleflash.vpcf")]
    [Hammer.Skip]
    public abstract partial class Weapon : BaseCarriable, ICarriableItem, IEntityHint
    {
        public Weapon() : base()
        {
            Type type = GetType();
            Info.LibraryName = Utils.GetLibraryName(type);

            WeaponAttribute weaponAttribute = Utils.GetAttribute<WeaponAttribute>(type);

            if (weaponAttribute != null)
            {
                WeaponInfo.Category = weaponAttribute.Category;

                Primary.AmmoType = weaponAttribute.PrimaryAmmoType;
                Primary.AmmoName = weaponAttribute.PrimaryAmmoName;

                if (Secondary != null)
                {
                    Secondary.AmmoType = weaponAttribute.SecondaryAmmoType;
                    Secondary.AmmoName = weaponAttribute.SecondaryAmmoName;
                }
            }

            EnableShadowInFirstPerson = false;

            Tags.Add(IItem.ITEM_TAG);
        }

        public void Equip(Player player) => OnEquip();

        public virtual void OnEquip() { }

        public void Remove() => OnRemove();

        public virtual void OnRemove() { }

        public string GetTranslationKey(string key) => Utils.GetTranslationKey(Info.LibraryName, key);

        public int AvailableAmmo(ClipInfo clipInfo)
        {
            if (Owner is not Player owner || clipInfo == null || clipInfo.AmmoName == null)
            {
                return 0;
            }

            return owner.Inventory.Ammo.Count(clipInfo.AmmoName);
        }

        public override void ActiveStart(Entity owner)
        {
            base.ActiveStart(owner);

            TimeSinceDeployed = 0;

            Primary.IsReloading = false;

            if (Secondary != null)
            {
                Secondary.IsReloading = false;
            }
        }

        public override void Spawn()
        {
            base.Spawn();

            Primary.ClipAmmo = Primary.ClipSize;

            SetModel(ModelPath);

            PickupTrigger = new();
            PickupTrigger.Parent = this;
            PickupTrigger.Position = Position;
            PickupTrigger.Rotation = Rotation;
        }

        public virtual void Reload(ClipInfo clipInfo)
        {
            if (clipInfo == null || WeaponInfo.Category == CarriableCategories.Melee || IsReloading || clipInfo.ClipAmmo >= clipInfo.ClipSize)
            {
                return;
            }

            TimeSinceReload = 0;

            if (Owner is Player player && !clipInfo.UnlimitedAmmo && (clipInfo.AmmoName == null || player.Inventory.Ammo.Count(clipInfo.AmmoName) <= 0))
            {
                return;
            }

            clipInfo.IsReloading = true;

            (Owner as AnimEntity).SetAnimParameter("b_reload", true);

            DoClientReload();
        }

		public virtual bool CanReload() => Owner.IsValid() && Input.Down(InputButton.Reload);

        public virtual void BaseSimulate(Client owner)
        {
            if (CanReload())
            {
                Reload(Primary);
            }

            //
            // Reload could have changed our owner
            //
            if (!Owner.IsValid())
            {
                return;
            }

            if (CanAttack(Primary, InputButton.Attack1))
            {
                using (LagCompensation())
                {
                    Primary.TimeSinceAttack = 0f;

                    Attack(Primary);
                }
            }

            //
            // AttackPrimary could have changed our owner
            //
            if (!Owner.IsValid())
            {
                return;
            }

            if (CanAttack(Secondary, InputButton.Attack2))
            {
                using (LagCompensation())
                {
                    Secondary.TimeSinceAttack = 0f;

                    Attack(Secondary);
                }
            }
        }

        public override void Simulate(Client owner)
        {
            if (TimeSinceDeployed < WeaponInfo.DeployTime)
            {
                return;
            }

            if (Input.Pressed(InputButton.Drop) && Input.Down(InputButton.Run) && Primary.ClipAmmo > 0 && !Primary.UnlimitedAmmo)
            {
                if (Primary.AmmoType != null && Primary.CanDropAmmo)
                {
                    if (IsServer)
                    {
                        Ammo ammoBox = Utils.GetObjectByType<Ammo>(Primary.AmmoType);
                        ammoBox.LastDropOwner = Owner;
                        ammoBox.SinceLastDrop = 0f;
                        ammoBox.Position = Owner.EyePosition + Owner.EyeRotation.Forward * AMMO_DROP_POSITION_OFFSET;
                        ammoBox.Rotation = Owner.EyeRotation;
                        ammoBox.Velocity = Owner.EyeRotation.Forward * AMMO_DROP_VELOCITY;
                        ammoBox.SetCurrentAmmo(Primary.ClipAmmo);
                    }

                    TakeAmmo(Primary, Primary.ClipAmmo);
                }
            }

            if (!IsReloading)
            {
                BaseSimulate(owner);
            }
            else if (TimeSinceReload > WeaponInfo.ReloadTime)
            {
                OnReloadFinish();
            }
        }

        public static int GetHoldType(CarriableCategories category)
        {
            return category switch
            {
                CarriableCategories.Melee => 0,
                CarriableCategories.Pistol => 1,
                CarriableCategories.SMG or CarriableCategories.Sniper => 2,
                CarriableCategories.Shotgun => 3,
                CarriableCategories.OffensiveEquipment => 0,
                CarriableCategories.UtilityEquipment => 0,
                CarriableCategories.Grenade => 0,
                _ => 0,
            };
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            anim.SetAnimParameter("holdtype", GetHoldType(WeaponInfo.Category));
            anim.SetAnimParameter("aim_body_weight", 1.0f);
        }

        public virtual bool CanAttack(ClipInfo clipInfo, InputButton inputButton)
		{
            if (clipInfo == null || !Owner.IsValid() || !Input.Down(inputButton) || TimeSinceDeployed <= WeaponInfo.DeployTime)
            {
                return false;
            }

            if (clipInfo.Rate <= 0)
            {
                return true;
            }

            return clipInfo.TimeSinceAttack > (1 / clipInfo.Rate);
		}

        public virtual void OnReloadFinish()
        {
            ClipInfo clipInfo = (Secondary?.IsReloading ?? false) ? Secondary : Primary;
            clipInfo.IsReloading = false;

            if (Owner is not Player player || clipInfo.AmmoName == null)
            {
                return;
            }

            if (!clipInfo.UnlimitedAmmo)
            {
                int ammo = player.Inventory.Ammo.Take(clipInfo.AmmoName, clipInfo.ClipSize - clipInfo.ClipAmmo);

                if (ammo == 0)
                {
                    return;
                }

                clipInfo.ClipAmmo += ammo;
            }
            else
            {
                clipInfo.ClipAmmo = clipInfo.ClipSize;
            }
        }

        [ClientRpc]
        public virtual void DoClientReload()
        {
            ViewModelEntity?.SetAnimParameter("reload", true);
        }

        [ClientRpc]
        protected virtual void FinishReload()
        {
            ViewModelEntity?.SetAnimParameter("reload_finished", true);
        }

        public virtual void Attack(ClipInfo clipInfo)
        {
            if (clipInfo == null)
            {
                return;
            }

            clipInfo.TimeSinceAttack = 0f;

            if (!TakeAmmo(clipInfo, 1))
            {
                PlaySound(clipInfo.DryFireSound).SetPosition(Position).SetVolume(0.2f);

                return;
            }

            (Owner as AnimEntity).SetAnimParameter("b_attack", true);

            if (IsClient)
            {
                ShootEffects(clipInfo);
            }

            PlaySound(clipInfo.ShootSound).SetPosition(Position).SetVolume(0.8f);

            for (int i = 0; i < clipInfo.Bullets; i++)
            {
                ShootBullet(clipInfo.Spread, clipInfo.Force, clipInfo.Damage, clipInfo.BulletSize);
            }
        }

        protected virtual void ShootEffects(ClipInfo clipInfo)
        {
            Host.AssertClient();

            if (clipInfo == null)
            {
                return;
            }

            if (WeaponInfo.Category != CarriableCategories.Melee)
            {
                foreach (ShootEffect shootEffect in clipInfo.ShootEffectList)
                {
                    Particles.Create(shootEffect.Name, EffectEntity, shootEffect.Attachment);
                }
            }

            if (IsLocalPawn)
            {
                using (Prediction.Off())
                {
                    _ = new Perlin(clipInfo.ShakeEffect.Length, clipInfo.ShakeEffect.Speed, clipInfo.ShakeEffect.Size, clipInfo.ShakeEffect.Rotation);
                }
            }

            ViewModelEntity?.SetAnimParameter("fire", true);
            CrosshairPanel?.CreateEvent("fire");
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

        public virtual IEnumerable<TraceResult> TraceBullet(Vector3 start, Vector3 end, float radius = 2.0f)
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

        public static bool TakeAmmo(ClipInfo clipInfo, int amount)
        {
            if (clipInfo.ClipAmmo < amount)
            {
                return false;
            }

            clipInfo.ClipAmmo -= amount;

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

        public override void CreateHudElements() { }

        public virtual void PickupStartTouch(Entity other)
        {
            if ((other != LastDropOwner || SinceLastDrop > 0.25f) && other is Player player)
            {
                LastDropOwner = null;

                player.Inventory.TryAdd(this);
            }
        }

        public virtual void PickupEndTouch(Entity other)
        {
            if (other is Player && LastDropOwner == other)
            {
                LastDropOwner = null;
            }
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
            LastDropOwner = Owner;
            SinceLastDrop = 0f;

            base.OnCarryDrop(dropper);

            if (PickupTrigger.IsValid())
            {
                PickupTrigger.EnableTouch = true;
            }
        }

        public virtual bool CanDrop { get; set; } = true;

        public float HintDistance => 80f;

        public TranslationData TextOnTick => new("WEAPON.USE", new TranslationData(GetTranslationKey("NAME")));

        public bool CanHint(Player client) => true;

        public EntityHintPanel DisplayHint(Player client) => new GlyphHint(TextOnTick, InputButton.Use);

        public void TextTick(Player player)
        {
            if (IsClient || player.LifeState != LifeState.Alive)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Use))
                {
                    ICarriableItem[] carriableItems = player.Inventory.GetSlotCarriable(WeaponInfo.Category);

                    if (carriableItems.Length > 0)
                    {
                        ICarriableItem carriableItem = carriableItems[0];

                        if (carriableItem is Weapon weapon)
                        {
                            player.Inventory.Drop(weapon);
                        }
                    }

                    player.Inventory.TryAdd(this, deleteIfFails: false, makeActive: true);
                }
            }
        }
    }
}
