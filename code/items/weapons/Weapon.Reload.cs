using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
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
    }
}
