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
                Primary.AmmoName = weaponAttribute.PrimaryAmmoName ?? Primary.AmmoName;

                if (Secondary != null)
                {
                    Secondary.AmmoName = weaponAttribute.SecondaryAmmoName ?? Secondary.AmmoName;
                }
            }

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

            PrimaryData.Reset(Primary);
            SecondaryData?.Reset(Secondary);
        }

        public virtual void Init()
        {
            ClipDataList.Clear();

            ClipDataList.Add(new());
            Primary.DataIndex = 0;

            if (Secondary != null)
            {
                ClipDataList.Add(new());
                Secondary.DataIndex = 1;
            }
        }

        public override void Spawn()
        {
            base.Spawn();

            Init();

            PrimaryData.Ammo = Primary.StartAmmo == -1 ? Primary.ClipSize : Primary.StartAmmo;

            if (Secondary != null)
            {
                SecondaryData.Ammo = Secondary.StartAmmo == -1 ? Secondary.ClipSize : Secondary.StartAmmo;
            }

            SetModel(ModelPath);

            PickupTrigger = new();
            PickupTrigger.Parent = this;
            PickupTrigger.Position = Position;
            PickupTrigger.Rotation = Rotation;

            EnableShadowInFirstPerson = false;
        }

        public static float GetRealRPM(int rpm) => 60f / rpm;

        public override void Simulate(Client owner)
        {
            if (TimeSinceDeployed < WeaponInfo.DeployTime)
            {
                return;
            }

            if (Input.Pressed(InputButton.Drop) && Input.Down(InputButton.Run) && PrimaryData.Ammo > 0 && !Primary.UnlimitedAmmo)
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
                            ammoBox.SetCurrentAmmo(PrimaryData.Ammo);
                        }

                        TakeAmmo(Primary, PrimaryData.Ammo);
                    }
                }
            }

            if (IsReloading && PrimaryData.TimeSinceReload >= Primary.ReloadTime)
            {
                OnReloadFinish(Primary);
            }

            if (!IsReloading || Primary.IsPartialReloading)
            {
                if (CanReload())
                {
                    Reload(Primary);
                }

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
            }

            if (!Owner.IsValid())
            {
                return;
            }

            if (!IsReloading || Secondary != null && Secondary.IsPartialReloading)
            {
                if (CanAttack(Secondary, InputButton.Attack2))
                {
                    using (LagCompensation())
                    {
                        Attack(Secondary);
                    }
                }
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

        public bool TakeAmmo(ClipInfo clipInfo, int amount)
        {
            if (clipInfo == null || ClipDataList[clipInfo.DataIndex].Ammo < amount)
            {
                return false;
            }

            ClipDataList[clipInfo.DataIndex].Ammo -= amount;

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

        public int GetClipInfoIndex(ClipInfo clipInfo)
        {
            if (clipInfo == Primary)
            {
                return 0;
            }
            else if (clipInfo == Secondary)
            {
                return 1;
            }

            return -1;
        }

        public ClipInfo GetClipInfoByIndex(int index)
        {
            return index switch
            {
                0 => Primary,
                1 => Secondary,
                _ => null
            };
        }
    }
}
