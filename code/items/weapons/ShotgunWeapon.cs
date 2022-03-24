namespace TTTReborn.Items
{
    public abstract partial class ShotgunWeapon : Weapon
    {
        public override bool IsPartialReloading { get; set; } = true;

        public ShotgunWeapon() : base() { }

        public override void OnReloadFinish()
        {
            ClipInfo clipInfo = (Secondary?.IsReloading ?? false) ? Secondary : Primary;
            clipInfo.IsReloading = false;

            if (Owner is not Player player || clipInfo.AmmoName == null || clipInfo.ClipAmmo >= clipInfo.ClipSize)
            {
                return;
            }

            int ammo = player.Inventory.Ammo.Take(clipInfo.AmmoName, 1);

            if (ammo == 0)
            {
                return;
            }

            clipInfo.ClipAmmo += ammo;

            if (clipInfo.ClipAmmo < clipInfo.ClipSize)
            {
                Reload(Primary);
            }
            else
            {
                FinishReload();
            }
        }
    }
}
