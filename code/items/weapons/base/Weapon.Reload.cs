using System;

using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        public virtual void Reload(ClipInfo clipInfo)
        {
            if (clipInfo == null || WeaponInfo.Category == CarriableCategories.Melee || IsReloading || clipInfo.Data.ClipAmmo >= clipInfo.ClipSize || clipInfo.ClipSize <= 0)
            {
                return;
            }

            if (Owner is Player player && !clipInfo.UnlimitedAmmo && (clipInfo.AmmoName == null || player.Inventory.Ammo.Count(clipInfo.AmmoName) <= 0))
            {
                return;
            }

            clipInfo.Data.IsReloading = true;
            clipInfo.Data.TimeSinceReload = 0f;

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

            clipInfo.Data.IsReloading = false;
            clipInfo.Data.TimeSinceReload = Math.Max(clipInfo.Data.TimeSinceReload, clipInfo.ReloadTime);

            if (Owner is not Player player || clipInfo.AmmoName == null)
            {
                return;
            }

            if (!clipInfo.UnlimitedAmmo)
            {
                int ammo = player.Inventory.Ammo.Take(clipInfo.AmmoName, Math.Min(clipInfo.ClipSize - clipInfo.Data.ClipAmmo, clipInfo.BulletsPerReload));

                if (ammo == 0)
                {
                    return;
                }

                clipInfo.Data.ClipAmmo += ammo;
            }
            else
            {
                clipInfo.Data.ClipAmmo = clipInfo.ClipSize;
            }

            if (clipInfo.Data.ClipAmmo < clipInfo.ClipSize)
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
