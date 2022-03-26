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

            ClipInfoData clipInfoData = GetClipInfoData(clipInfo);

            if (clipInfoData == null || WeaponInfo.Category == CarriableCategories.Melee || IsReloading || clipInfoData.ClipAmmo >= clipInfo.ClipSize || clipInfo.ClipSize <= 0)
            {
                return;
            }

            if (Owner is Player player && !clipInfo.UnlimitedAmmo && (clipInfo.AmmoName == null || player.Inventory.Ammo.Count(clipInfo.AmmoName) <= 0))
            {
                return;
            }

            clipInfoData.TimeSinceReload = 0f;
            clipInfoData.IsReloading = true;

            (Owner as AnimEntity).SetAnimParameter(clipInfo.ReloadAnim, true);

            ClientReload(GetClipInfoIndex(clipInfo));
        }

        public virtual bool CanReload() => !IsReloading && Owner.IsValid() && Input.Down(InputButton.Reload);

        public virtual void OnReloadFinish(ClipInfo clipInfo)
        {
            ClipInfoData clipInfoData = GetClipInfoData(clipInfo);

            if (clipInfoData == null)
            {
                return;
            }

            clipInfoData.TimeSinceReload = Math.Max(clipInfoData.TimeSinceReload, clipInfo.ReloadTime);
            clipInfoData.IsReloading = false;

            if (Owner is not Player player || clipInfo.AmmoName == null)
            {
                return;
            }

            if (!clipInfo.UnlimitedAmmo)
            {
                int ammo = player.Inventory.Ammo.Take(clipInfo.AmmoName, Math.Min(clipInfo.ClipSize - clipInfoData.ClipAmmo, clipInfo.BulletsPerReload));

                if (ammo == 0)
                {
                    return;
                }

                clipInfoData.ClipAmmo += ammo;
            }
            else
            {
                clipInfoData.ClipAmmo = clipInfo.ClipSize;
            }

            if (clipInfoData.ClipAmmo < clipInfo.ClipSize)
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
