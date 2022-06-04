using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class MeleeWeapon : Weapon
    {
        public virtual int MeleeDistance { get; } = 80;

        public virtual void MeleeStrike(float damage, float force)
        {
            Vector3 forward = Owner.EyeRotation.Forward;
            forward = forward.Normal;

            foreach (TraceResult tr in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * MeleeDistance, 10f))
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
                    DamageInfo damageInfo = DamageInfo.FromBullet(tr.EndPosition, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Entity.TakeDamage(damageInfo);
                }
            }
        }

        public override void Attack(ClipInfo clipInfo)
        {
            (Owner as AnimatedEntity).SetAnimParameter("b_attack", true);

            if (IsClient)
            {
                ShootEffects(GetClipInfoIndex(clipInfo));
            }

            PlaySound("rust_boneknife.attack");
            MeleeStrike(clipInfo.Damage, 1.5f);
        }
    }
}
