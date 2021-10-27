using System;

using Sandbox;
using Sandbox.ScreenShake;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_shotgun")]
    [Weapon(SlotType = SlotType.Primary, AmmoType = "buckshot")]
    [Spawnable]
    [Buyable(Price = 100)]
    [Hammer.EditorModel("weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl")]
    public partial class Shotgun : TTTWeapon
    {
        public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
        public override float PrimaryRate => 1;
        public override float SecondaryRate => 1;
        public override Type AmmoEntity => typeof(BuckshotAmmo);
        public override int ClipSize => 8;
        public override float ReloadTime => 0.5f;
        public override float DeployTime => 0.6f;
        public override int BaseDamage => 6; // This is per bullet, so 6 x 10 for the shotgun.

        public override void Spawn()
        {
            base.Spawn();

            SetModel("weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl");
        }

        public override void AttackPrimary()
        {
            if (!TakeAmmo(1))
            {
                PlaySound("pistol.dryfire").SetPosition(Position).SetVolume(0.2f);

                return;
            }

            (Owner as AnimEntity).SetAnimBool("b_attack", true);

            ShootEffects();

            PlaySound("rust_pumpshotgun.shoot").SetPosition(Position).SetVolume(0.8f);

            for (int i = 0; i < 10; i++)
            {
                ShootBullet(0.15f, 0.3f, BaseDamage, 3.0f);
            }
        }

        [ClientRpc]
        protected override void ShootEffects()
        {
            Host.AssertClient();

            Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
            Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

            ViewModelEntity?.SetAnimBool("fire", true);
            CrosshairPanel?.CreateEvent("fire");

            if (IsLocalPawn)
            {
                new Perlin(1.0f, 1.5f, 2.0f);
            }
        }

        public override void OnReloadFinish()
        {
            IsReloading = false;

            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

            if (AmmoClip >= ClipSize)
            {
                return;
            }

            if (Owner is TTTPlayer player)
            {
                int ammo = player.Inventory.Ammo.Take(AmmoType, 1);

                if (ammo == 0)
                {
                    return;
                }

                AmmoClip += ammo;

                if (AmmoClip < ClipSize)
                {
                    Reload();
                }
                else
                {
                    FinishReload();
                }
            }
        }

        [ClientRpc]
        protected virtual void FinishReload()
        {
            ViewModelEntity?.SetAnimBool("reload_finished", true);
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            anim.SetParam("holdtype", 3);
            anim.SetParam("aimat_weight", 1.0f);
        }
    }
}
