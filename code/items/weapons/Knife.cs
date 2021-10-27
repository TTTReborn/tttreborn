using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_knife")]
    [Weapon(SlotType = SlotType.Melee)]
    [Buyable(Price = 100)]
    [Hammer.EditorModel("weapons/rust_boneknife/rust_boneknife.vmdl")]
    partial class Knife : TTTWeapon
    {
        public override string ViewModelPath => "weapons/rust_boneknife/v_rust_boneknife.vmdl";
        public override float PrimaryRate => 1.0f;
        public override float DeployTime => 0.2f;
        public override int BaseDamage => 45;
        public virtual int MeleeDistance => 80;

        public override void Spawn()
        {
            base.Spawn();

            SetModel("weapons/rust_boneknife/rust_boneknife.vmdl");
        }

        public virtual void MeleeStrike(float damage, float force)
        {
            Vector3 forward = Owner.EyeRot.Forward;
            forward = forward.Normal;

            foreach (TraceResult tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * MeleeDistance, 10f))
            {
                if (!tr.Entity.IsValid())
                {
                    continue;
                }

                tr.Surface.DoBulletImpact(tr);

                if (!IsServer)
                {
                    continue;
                }

                using (Prediction.Off())
                {
                    DamageInfo damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Entity.TakeDamage(damageInfo);
                }
            }
        }

        public override void AttackPrimary()
        {
            (Owner as AnimEntity).SetAnimBool("b_attack", true);

            ShootEffects();

            PlaySound("rust_boneknife.attack");
            MeleeStrike(BaseDamage, 1.5f);
        }
    }
}
