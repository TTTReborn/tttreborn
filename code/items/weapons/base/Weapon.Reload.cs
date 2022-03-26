using System;

using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        public virtual void Reload(ClipInfo clipInfo)
        {
            if (clipInfo == null)
            {
                return;
            }

            bool isPrimary = clipInfo == Primary;

            if (WeaponInfo.Category == CarriableCategories.Melee || IsReloading || (isPrimary ? PrimaryClipAmmo : SecondaryClipAmmo) >= clipInfo.ClipSize || clipInfo.ClipSize <= 0)
            {
                return;
            }

            if (Owner is Player player && !clipInfo.UnlimitedAmmo && (clipInfo.AmmoName == null || player.Inventory.Ammo.Count(clipInfo.AmmoName) <= 0))
            {
                return;
            }

            if (isPrimary)
            {
                IsPrimaryReloading = true;
                TimeSincePrimaryReload = 0f;
            }
            else
            {
                IsSecondaryReloading = true;
                TimeSinceSecondaryReload = 0f;
            }

            (Owner as AnimEntity).SetAnimParameter(clipInfo.ReloadAnim, true);

            ClientReload(GetClipInfoIndex(clipInfo));
        }

        public virtual bool CanReload() => !IsReloading && Owner.IsValid() && Input.Down(InputButton.Reload);

        public virtual void OnReloadFinish(ClipInfo clipInfo)
        {
            if (clipInfo == null)
            {
                return;
            }

            bool isPrimary = clipInfo == Primary;

            if (isPrimary)
            {
                IsPrimaryReloading = false;
                TimeSincePrimaryReload = Math.Max(TimeSincePrimaryReload, clipInfo.ReloadTime);
            }
            else
            {
                IsSecondaryReloading = false;
                TimeSinceSecondaryReload = Math.Max(TimeSinceSecondaryReload, clipInfo.ReloadTime);
            }

            if (Owner is not Player player || clipInfo.AmmoName == null)
            {
                return;
            }

            if (!clipInfo.UnlimitedAmmo)
            {
                int ammo = player.Inventory.Ammo.Take(clipInfo.AmmoName, Math.Min(clipInfo.ClipSize - (isPrimary ? PrimaryClipAmmo : SecondaryClipAmmo), clipInfo.BulletsPerReload));

                if (ammo == 0)
                {
                    return;
                }

                if (isPrimary)
                {
                    PrimaryClipAmmo += ammo;
                }
                else
                {
                    SecondaryClipAmmo += ammo;
                }
            }
            else
            {
                if (isPrimary)
                {
                    PrimaryClipAmmo = clipInfo.ClipSize;
                }
                else
                {
                    SecondaryClipAmmo = clipInfo.ClipSize;
                }
            }

            if ((isPrimary ? PrimaryClipAmmo : SecondaryClipAmmo) < clipInfo.ClipSize)
            {
                Reload(clipInfo);
            }
            else
            {
                ClientFinishReload(GetClipInfoIndex(clipInfo));
            }
        }

        [ClientRpc]
        public virtual void ClientReload(int clipInfoIndex)
        {
            ClipInfo clipInfo = GetClipInfoByIndex(clipInfoIndex);

            if (!string.IsNullOrEmpty(clipInfo.ViewModelReloadAnim))
            {
                ViewModelEntity?.SetAnimParameter(clipInfo.ViewModelReloadAnim, true);
            }
        }

        [ClientRpc]
        public virtual void ClientFinishReload(int clipInfoIndex)
        {
            ClipInfo clipInfo = GetClipInfoByIndex(clipInfoIndex);

            if (!string.IsNullOrEmpty(clipInfo.ViewModelReloadFinishAnim))
            {
                ViewModelEntity?.SetAnimParameter(clipInfo.ViewModelReloadFinishAnim, true);
            }
        }
    }
}
