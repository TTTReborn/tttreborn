using System;

using Sandbox;

using TTTReborn.Entities;

namespace TTTReborn.Items
{
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
                Primary.AmmoName = weaponAttribute.PrimaryAmmoName;

                if (Secondary != null)
                {
                    Secondary.AmmoName = weaponAttribute.SecondaryAmmoName;
                }
            }

            Primary.ClipAmmo = Primary.ClipSize;
            IsPartialReloading = false;

            EnableShadowInFirstPerson = false;

            Tags.Add(IItem.ITEM_TAG);
        }

        public void Equip(Player player) => OnEquip();

        public virtual void OnEquip() { }

        public void Remove() => OnRemove();

        public virtual void OnRemove() { }

        public string GetTranslationKey(string key) => Utils.GetTranslationKey(Info.LibraryName, key);

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

            SetModel(ModelPath);

            PickupTrigger = new();
            PickupTrigger.Parent = this;
            PickupTrigger.Position = Position;
            PickupTrigger.Rotation = Rotation;
        }

        public static float GetRealRPM(int rpm) => 60f / rpm;

        public override void Simulate(Client owner)
        {
            if (TimeSinceDeployed < WeaponInfo.DeployTime)
            {
                return;
            }

            if (Input.Pressed(InputButton.Drop) && Input.Down(InputButton.Run) && Primary.ClipAmmo > 0 && !Primary.UnlimitedAmmo)
            {
                if (Primary.AmmoName != null && Primary.CanDropAmmo)
                {
                    Type type = Utils.GetTypeByLibraryName<Ammo>(Primary.AmmoName);

                    if (type != null)
                    {
                        if (IsServer)
                        {
                            Ammo ammoBox = Utils.GetObjectByType<Ammo>(type);
                            ammoBox.LastDropOwner = Owner;
                            ammoBox.TimeSinceLastDrop = 0f;
                            ammoBox.Position = Owner.EyePosition + Owner.EyeRotation.Forward * AMMO_DROP_POSITION_OFFSET;
                            ammoBox.Rotation = Owner.EyeRotation;
                            ammoBox.Velocity = Owner.EyeRotation.Forward * AMMO_DROP_VELOCITY;
                            ammoBox.SetCurrentAmmo(Primary.ClipAmmo);
                        }

                        TakeAmmo(Primary, Primary.ClipAmmo);
                    }
                }
            }

            if (!IsReloading || IsPartialReloading)
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
                        Attack(Secondary);
                    }
                }
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

        public int AvailableAmmo(ClipInfo clipInfo)
        {
            if (Owner is not Player owner || clipInfo == null || clipInfo.AmmoName == null)
            {
                return 0;
            }

            return owner.Inventory.Ammo.Count(clipInfo.AmmoName);
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
    }
}
