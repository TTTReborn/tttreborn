using System;
using System.Reflection;

using Sandbox;

using TTTReborn.Entities;

namespace TTTReborn.Items
{
    public abstract partial class Weapon : BaseCarriable, ICarriableItem, IEntityHint
    {
        public Weapon() : base()
        {
            Type type = GetType();

            Info.LibraryName = Utils.GetLibraryName(type);

            WeaponAttribute weaponAttribute = Utils.GetAttribute<WeaponAttribute>(type);

            if (weaponAttribute != null)
            {
                CarriableInfo.Category = weaponAttribute.Category;
                Primary.AmmoName = (weaponAttribute.PrimaryAmmoType != null ? Utils.GetLibraryName(weaponAttribute.PrimaryAmmoType) : null) ?? Primary.AmmoName;

                if (Secondary != null)
                {
                    Secondary.AmmoName = (weaponAttribute.PrimaryAmmoType != null ? Utils.GetLibraryName(weaponAttribute.SecondaryAmmoType) : null) ?? Secondary.AmmoName;
                }
            }

            Tags.Add(IItem.ITEM_TAG);

            Init();

            CurrentClip = Primary;
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

            IsReloading = false;
            TimeSinceReload = GetClipInfoMax<float>("ReloadTime");
        }

        public virtual void Init()
        {
            Primary.Ammo = Primary.StartAmmo == -1 ? Primary.ClipSize : Primary.StartAmmo;

            if (Secondary != null)
            {
                Secondary.Ammo = Secondary.StartAmmo == -1 ? Secondary.ClipSize : Secondary.StartAmmo;
            }
        }

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);

            PickupTrigger = new()
            {
                Parent = this,
                Position = Position,
                Rotation = Rotation
            };

            EnableShadowInFirstPerson = false;
        }

        public static float GetRealRPM(int rpm) => 60f / rpm;

        public override void Simulate(IClient owner)
        {
            if (TimeSinceDeployed < WeaponInfo.DeployTime)
            {
                return;
            }

            ResetBurstFireCount(CurrentClip, InputButton.PrimaryAttack);

            if (Input.Pressed(InputButton.Drop) && Input.Down(InputButton.Run) && ClipAmmo > 0 && !CurrentClip.UnlimitedAmmo)
            {
                if (CurrentClip.AmmoName != null && CurrentClip.CanDropAmmo)
                {
                    Type type = Utils.GetTypeByLibraryName<Ammo>(CurrentClip.AmmoName);

                    if (type != null)
                    {
                        if (Game.IsServer)
                        {
                            Ammo ammoBox = Utils.GetObjectByType<Ammo>(type);
                            ammoBox.LastDropOwner = Owner;
                            ammoBox.TimeSinceLastDrop = 0f;
                            ammoBox.Position = Owner.AimRay.Position + Owner.AimRay.Forward * AMMO_DROP_POSITION_OFFSET;
                            ammoBox.Rotation = Owner.Rotation;
                            ammoBox.Velocity = Owner.AimRay.Forward * AMMO_DROP_VELOCITY;
                            ammoBox.SetCurrentAmmo(ClipAmmo);
                        }

                        TakeAmmo(CurrentClip, ClipAmmo);
                    }
                }
            }

            if (IsReloading && TimeSinceReload >= CurrentReloadTime)
            {
                OnReloadFinish(CurrentClip);
            }

            if (!IsReloading || CurrentClip.IsPartialReloading)
            {
                if (CanReload())
                {
                    Reload(CurrentClip);
                }

                if (!Owner.IsValid())
                {
                    return;
                }

                if (CanAttack(CurrentClip, InputButton.PrimaryAttack))
                {
                    using (LagCompensation())
                    {
                        Attack(CurrentClip);
                    }
                }
            }

            if (!Owner.IsValid())
            {
                return;
            }

            if (Secondary != null && (!IsReloading || CurrentClip.IsPartialReloading) && !CanZoom)
            {
                CurrentClip = Secondary;

                if (CanAttack(CurrentClip, InputButton.SecondaryAttack))
                {
                    using (LagCompensation())
                    {
                        Attack(CurrentClip);
                    }
                }

                CurrentClip = Primary;
            }

            // TODO Zoom
        }

        public static CitizenAnimationHelper.HoldTypes GetHoldType(CarriableCategories category) => category switch
        {
            CarriableCategories.Melee => CitizenAnimationHelper.HoldTypes.None,
            CarriableCategories.Pistol => CitizenAnimationHelper.HoldTypes.Pistol,
            CarriableCategories.SMG or CarriableCategories.Sniper => CitizenAnimationHelper.HoldTypes.Rifle,
            CarriableCategories.Shotgun => CitizenAnimationHelper.HoldTypes.Shotgun,
            CarriableCategories.OffensiveEquipment => CitizenAnimationHelper.HoldTypes.None,
            CarriableCategories.UtilityEquipment => CitizenAnimationHelper.HoldTypes.None,
            CarriableCategories.Grenade => CitizenAnimationHelper.HoldTypes.None,
            _ => 0,
        };

        public override void SimulateAnimator(CitizenAnimationHelper anim)
        {
            anim.HoldType = GetHoldType(CarriableInfo.Category);
            anim.AimBodyWeight = 1.0f;
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
            if (clipInfo == null || ClipAmmo < amount)
            {
                return false;
            }

            clipInfo.Ammo -= amount;

            if (clipInfo == CurrentClip)
            {
                ClipAmmo -= amount;

                if (IsAutoReload && ClipAmmo == 0 && AvailableAmmo(clipInfo) > 0)
                {
                    Reload(clipInfo);
                }
            }

            return true;
        }

        public bool GiveAmmo(ClipInfo clipInfo, int amount)
        {
            if (clipInfo == null)
            {
                return false;
            }

            clipInfo.Ammo += amount;

            if (clipInfo == CurrentClip)
            {
                ClipAmmo += amount;
            }

            return true;
        }

        public override void CreateViewModel()
        {
            Game.AssertClient();

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

        public virtual int GetClipInfoIndex(ClipInfo clipInfo)
        {
            for (int i = 0; i < ClipInfos.Length; i++)
            {
                if (clipInfo == ClipInfos[i])
                {
                    return i;
                }
            }

            return -1;
        }

        public virtual T GetClipInfoMax<T>(string propertyName) where T : IComparable
        {
            //if (ClipInfos.Length < 1)
            //{
                return default;
            //}

            //PropertyInfo propertyInfo = ClipInfos[0].GetType().GetProperty(propertyName);
            //T highest = (T) propertyInfo.GetValue(ClipInfos[0], null);

            //if (ClipInfos.Length > 1)
            //{
            //    for (int i = 1; i < ClipInfos.Length; i++)
            //    {
            //        T tmp = (T) propertyInfo.GetValue(ClipInfos[i], null);

            //        if (tmp.CompareTo(highest) > 0)
            //        {
            //            highest = tmp;
            //        }
            //    }
            //}

            //return highest;
        }
    }
}
