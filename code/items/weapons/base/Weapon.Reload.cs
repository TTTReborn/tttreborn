using System;

using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        public virtual void Reload(ClipInfo clipInfo)
        {
            if (clipInfo == null || CarriableInfo.Category == CarriableCategories.Melee || IsReloading || ClipAmmo >= clipInfo.ClipSize || clipInfo.ClipSize <= 0)
            {
                return;
            }

            if (Owner is Player player && !clipInfo.UnlimitedAmmo && (clipInfo.AmmoName == null || player.Inventory.Ammo.Count(clipInfo.AmmoName) <= 0))
            {
                return;
            }

            IsReloading = true;
            TimeSinceReload = 0f;
            CurrentReloadTime = clipInfo.ReloadTime;

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

            IsReloading = false;
            TimeSinceReload = Math.Max(TimeSinceReload, clipInfo.ReloadTime);

            if (Owner is not Player player || clipInfo.AmmoName == null || ClipAmmo >= clipInfo.ClipSize)
            {
                return;
            }

            int reloadBullets = Math.Min(clipInfo.ClipSize - ClipAmmo, clipInfo.BulletsPerReload);
            int ammo = clipInfo.UnlimitedAmmo ? reloadBullets : player.Inventory.Ammo.Take(clipInfo.AmmoName, reloadBullets);

            if (ammo > 0)
            {
                ClipAmmo += ammo;
            }

            if (ammo > 0 && ClipAmmo < clipInfo.ClipSize)
            {
                Reload(clipInfo);
            }
            else
            {
                FinishReload(clipInfo);
            }
        }

        public virtual void FinishReload(ClipInfo clipInfo)
        {
            ClientFinishReload(GetClipInfoIndex(clipInfo));
        }

        [ClientRpc]
        public virtual void ClientReload(int clipInfoIndex)
        {
            if (clipInfoIndex < 0 || clipInfoIndex >= ClipInfos.Length)
            {
                return;
            }

            ClipInfo clipInfo = ClipInfos[clipInfoIndex];

            if (!string.IsNullOrEmpty(clipInfo.ViewModelReloadAnim))
            {
                ViewModelEntity?.SetAnimParameter(clipInfo.ViewModelReloadAnim, true);
            }
        }

        [ClientRpc]
        public virtual void ClientFinishReload(int clipInfoIndex)
        {
            if (clipInfoIndex < 0 || clipInfoIndex >= ClipInfos.Length)
            {
                return;
            }

            ClipInfo clipInfo = ClipInfos[clipInfoIndex];

            if (!string.IsNullOrEmpty(clipInfo.ViewModelReloadFinishAnim))
            {
                ViewModelEntity?.SetAnimParameter(clipInfo.ViewModelReloadFinishAnim, true);
            }
        }
    }
}
