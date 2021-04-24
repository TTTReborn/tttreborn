using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTTGamemode
{
    public partial class Player : BasePlayer
    {
        public enum Role { None, Innocent, Detective, Traitor }

        public Body Body { get; set; }
        public Role Role { get; set; } = Player.Role.None;
        public int Credits { get; set; } = 0;

        private TimeSince _timeSinceDropped;
        private DamageInfo _lastDamageInfo;

        public Player()
        {
            Inventory = new Inventory(this);
            Animator = new StandardPlayerAnimator();

            Role = Role.None;
            Credits = 0;
        }

        public bool IsSpectator
        {
            get => (Camera is SpectateCamera);
        }

        public void MakeSpectator(Vector3 position = default)
        {
            EnableAllCollisions = false;
            EnableDrawing = false;
            Controller = null;
            Camera = new SpectateCamera
            {
                DeathPosition = position,
                TimeSinceDied = 0
            };
        }

        public override void Respawn()
        {
            RemoveBodyEntity();

            base.Respawn();
        }

        public override void OnKilled()
        {
            base.OnKilled();

            CreateBodyOnServer(_lastDamageInfo.Force, GetHitboxBone(_lastDamageInfo.HitboxIndex));
            Inventory.DeleteContents();
            MakeSpectator();
        }

        protected override void Tick()
        {
            TickActiveChild();

            if (Input.ActiveChild != null)
            {
                ActiveChild = Input.ActiveChild;
            }

            if (LifeState != LifeState.Alive)
                return;

            TickPlayerUse();

            if (IsServer)
            {
                using (Prediction.Off())
                {
                    TickInspectBody();
                }
            }
        }

        protected override void UseFail()
        {
            // Do nothing. By default this plays a sound that we don't want.
        }

        public override void StartTouch(Entity other)
        {
            if (_timeSinceDropped < 1)
                return;

            base.StartTouch(other);
        }

        private void TickInspectBody()
        {
            if (!Input.Pressed(InputButton.Use))
                return;

            var trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * 80f)
                .HitLayer(CollisionLayer.Debris)
                .Ignore(ActiveChild)
                .Ignore(this)
                .Radius(2)
                .Run();

            if (trace.Hit && trace.Entity is Body body && body.Player != null)
            {
                // Scoop up the credits on the body
                if (Role == Role.Traitor)
                {
                    Credits += body.Player.Credits;
                    body.Player.Credits = 0;
                }

                // Allow traitors to inspect body without identifying it by holding crouch
                if (Role != Role.Traitor || !Input.Down(InputButton.Crouch))
                {
                    body.Identified = true;
                }

                InspectedBody(body);
            }
        }

        public override void TakeDamage(DamageInfo info)
        {
            // Headshot deals x2 damage
            if (info.HitboxIndex == 0)
            {
                info.Damage *= 2.0f;
            }

            if (info.Attacker is Player attacker && attacker != this)
            {
                attacker.DidDamage(info.Position, info.Damage, ((float) Health).LerpInverse(100, 0));
            }

            TookDamage(info.Weapon.IsValid() ? info.Weapon.WorldPos : info.Attacker.WorldPos);

            // Play pain sounds
            if ((info.Flags & DamageFlags.Fall) == DamageFlags.Fall)
            {
                PlaySound("fall");
            }
            else if ((info.Flags & DamageFlags.Bullet) == DamageFlags.Bullet)
            {
                PlaySound("grunt" + Rand.Int(1, 4));
            }

            // Register player damage with the Karma system
            Game.Instance?.Karma?.RegisterPlayerDamage(info.Attacker, this, info.Damage);

            _lastDamageInfo = info;

            base.TakeDamage(info);
        }

        private void CreateBodyOnServer(Vector3 force, int forceBone)
        {
            var ragdoll = new PlayerCorpse
            {
                Pos = Pos,
                Rot = Rot
            };

            ragdoll.CopyFrom(this);
            ragdoll.ApplyForceToBone(force, forceBone);
            ragdoll.Player = this;

            Body = ragdoll;
        }

        public void RemoveBodyEntity()
        {
            if (Body != null && Body.IsValid())
            {
                Body.Delete();
                Body = null;
            }
        }

        [ClientRpc]
        public void DidDamage(Vector3 position, float amount, float inverseHealth)
        {
            Sound.FromScreen("dm.ui_attacker")
                .SetPitch(1 + inverseHealth * 1);

            HitIndicator.Current?.OnHit(position, amount);
        }

        [ClientRpc]
        public void TookDamage(Vector3 position)
        {
            DamageIndicator.Current?.OnHit(position);
        }

        [ClientRpc]
        public void InspectedBody(Body body)
        {
            
        }

        protected override void OnRemove()
        {
            RemoveBodyEntity();

            base.OnRemove();
        }
    }
}
