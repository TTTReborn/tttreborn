using Sandbox;

namespace TTTGamemode
{
    partial class Weapon : BaseWeapon
    {
        public virtual AmmoType AmmoType => AmmoType.Pistol;
        public virtual int ClipSize => 16;
        public virtual float ReloadTime => 3.0f;
        public virtual bool IsMelee => false;
        public virtual int Bucket => 1;
        public virtual int BucketWeight => 100;
        public virtual bool UnlimitedAmmo => false;
        public virtual bool HasFlashlight => false;
        public virtual int HoldType => 1;

        [Net, Predicted]
        public int AmmoClip { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceReload { get; set; }

        [Net, Predicted]
        public bool IsReloading { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceDeployed { get; set; }

        protected SpotLight _flashlight;

        public int AvailableAmmo()
        {
            if (Owner is not TTTPlayer owner)
                return 0;

            return owner.AmmoCount(AmmoType);
        }

        public override void ActiveStart(Entity owner)
        {
            base.ActiveStart(owner);

            TimeSinceDeployed = 0;

            ShowFlashlight(false);
        }

        public override void ActiveEnd(Entity owner, bool wasDropped)
        {
            ShowFlashlight(false);

            base.ActiveEnd(owner, wasDropped);
        }

        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

        public override void Spawn()
        {
            base.Spawn();

            AmmoClip = ClipSize;

            SetModel("weapons/rust_pistol/rust_pistol.vmdl");
        }

        public override void Reload(Sandbox.Player owner)
        {
            if (IsMelee || IsReloading)
                return;

            if (AmmoClip >= ClipSize)
                return;

            if (!UnlimitedAmmo && AvailableAmmo() <= 0)
                return;

            TimeSinceReload = 0;
            IsReloading = true;

            Owner.SetAnimParam("b_reload", true);

            DoClientReload();
        }

        public override void TickPlayerAnimator(PlayerAnimator anim)
        {
            anim.SetParam("holdtype", HoldType);
            anim.SetParam("aimat_weight", 1.0f);
        }

        public override void OnPlayerControlTick(Sandbox.Player owner)
        {
            if (TimeSinceDeployed < 0.6f)
                return;

            if (!IsReloading)
            {
                base.OnPlayerControlTick(owner);
            }
            else if (TimeSinceReload > ReloadTime)
            {
                OnReloadFinish();
            }
        }

        public virtual void OnReloadFinish()
        {
            IsReloading = false;

            if (Owner is TTTPlayer player)
            {
                if (!UnlimitedAmmo)
                {
                    var ammo = player.TakeAmmo(AmmoType, ClipSize - AmmoClip);

                    if (ammo == 0)
                        return;

                    AmmoClip += ammo;
                }
                else
                {
                    AmmoClip = ClipSize;
                }
            }
        }

        [ClientRpc]
        public virtual void DoClientReload()
        {
            ViewModelEntity?.SetAnimParam("reload", true);
        }

        public override void AttackPrimary(Sandbox.Player owner)
        {
            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

            ShootEffects();
            ShootBullet(0.05f, 1.5f, 9.0f, 3.0f);
        }

        [ClientRpc]
        protected virtual void ShootEffects()
        {
            Host.AssertClient();

            if (!IsMelee)
            {
                Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
            }

            if (Owner == Sandbox.Player.Local)
            {
                _ = new Sandbox.ScreenShake.Perlin();
            }

            ViewModelEntity?.SetAnimParam("fire", true);
            CrosshairPanel?.OnEvent("fire");
        }

        public virtual void ShootBullet(float spread, float force, float damage, float bulletSize)
        {
            var forward = Owner.EyeRot.Forward;
            forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
            forward = forward.Normal;

            foreach (var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * 5000, bulletSize))
            {
                tr.Surface.DoBulletImpact(tr);

                if (!IsServer || !tr.Entity.IsValid())
                    continue;

                using (Prediction.Off())
                {
                    var damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Entity.TakeDamage(damageInfo);
                }
            }
        }

        public void ShowFlashlight(bool shouldShow)
        {
            if (HasFlashlight && shouldShow)
            {
                // Create or enable the spot light.
            }
            else
            {
                // Disable the spot light.
            }
        }

        public bool TakeAmmo(int amount)
        {
            if (AmmoClip < amount)
                return false;

            AmmoClip -= amount;

            return true;
        }

        public override void CreateViewModel()
        {
            Host.AssertClient();

            if (string.IsNullOrEmpty(ViewModelPath))
                return;

            ViewModelEntity = new ViewModel
            {
                WorldPos = WorldPos,
                Owner = Owner,
                EnableViewmodelRendering = true
            };

            ViewModelEntity.SetModel(ViewModelPath);
        }

        public override void CreateHudElements()
        {
            if (Sandbox.Hud.CurrentPanel == null)
                return;

            CrosshairPanel = new Crosshair
            {
                Parent = Sandbox.Hud.CurrentPanel
            };

            CrosshairPanel.AddClass(ClassInfo.Name);
        }

        public bool IsUsable()
        {
            if (IsMelee || ClipSize == 0 || AmmoClip > 0)
            {
                return true;
            }

            return AvailableAmmo() > 0;
        }
    }
}
