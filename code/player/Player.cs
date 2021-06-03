using Sandbox;

using TTTReborn.Gamemode;
using TTTReborn.Player.Camera;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : Sandbox.Player
    {
        public enum RoleType { None, Innocent, Detective, Traitor }

        public Body Body { get; set; }
        public RoleType Role { get; set; }
        public int Credits { get; set; } = 0;

        private TimeSince _timeSinceDropped;
        private DamageInfo _lastDamageInfo;

        public TTTPlayer()
        {
            Inventory = new Inventory(this);

            Role = RoleType.None;
            Credits = 0;
        }

        public override void Respawn()
        {
            SetModel("models/citizen/citizen.vmdl");

            Controller = new WalkController();
            Animator = new StandardPlayerAnimator();
            Camera = new FirstPersonCamera();

            EnableAllCollisions = true;
            EnableDrawing = true;
            EnableHideInFirstPerson = true;
            EnableShadowInFirstPerson = true;

            base.Respawn();
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

        public override void OnKilled()
        {
            base.OnKilled();

            CreateBodyOnServer(_lastDamageInfo.Force, GetHitboxBone(_lastDamageInfo.HitboxIndex));
            Inventory.DeleteContents();
            MakeSpectator();
        }

        public override void Simulate(Client client)
        {
            SimulateActiveChild(client, ActiveChild);

            if (Input.ActiveChild != null)
            {
                ActiveChild = Input.ActiveChild;
            }

            if (LifeState != LifeState.Alive)
            {
                return;
            }

            TickPlayerUse();

            PawnController controller = GetActiveController();
            controller?.Simulate(client, this, GetActiveAnimator());
        }

        protected override void UseFail()
        {
            // Do nothing. By default this plays a sound that we don't want.
        }

        public override void StartTouch(Entity other)
        {
            if (_timeSinceDropped < 1)
            {
                return;
            }

            base.StartTouch(other);
        }

        private void TickInspectBody()
        {
            if (!Input.Pressed(InputButton.Use))
            {
                return;
            }

            TraceResult trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * 80f)
                .HitLayer(CollisionLayer.Debris)
                .Ignore(ActiveChild)
                .Ignore(this)
                .Radius(2)
                .Run();

            if (trace.Hit && trace.Entity is Body body && body.Player != null)
            {
                // Scoop up the credits on the body
                if (Role == RoleType.Traitor)
                {
                    Credits += body.Player.Credits;
                    body.Player.Credits = 0;
                }

                // Allow traitors to inspect body without identifying it by holding crouch
                if (Role != RoleType.Traitor || !Input.Down(InputButton.Duck))
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

            if (info.Attacker is TTTPlayer attacker && attacker != this)
            {
                attacker.DidDamage(info.Position, info.Damage, ((float)Health).LerpInverse(100, 0));
            }

            if (info.Weapon != null)
            {
                TookDamage(info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position);
            }

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
            TTTReborn.Gamemode.Game.Instance?.Karma?.RegisterPlayerDamage(info.Attacker as TTTPlayer, this, info.Damage);

            _lastDamageInfo = info;

            base.TakeDamage(info);
        }

        private void CreateBodyOnServer(Vector3 force, int forceBone)
        {
            // TODO: Create a ragdoll.
            // var ragdoll = new PlayerCorpse
            // {
            //     Pos = Pos,
            //     Rot = Rot
            // };
            //
            // ragdoll.CopyFrom(this);
            // ragdoll.ApplyForceToBone(force, forceBone);
            // ragdoll.Player = this;
            //
            // Body = ragdoll;
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
        }

        [ClientRpc]
        public void TookDamage(Vector3 position)
        {

        }

        [ClientRpc]
        public void InspectedBody(Body body)
        {

        }

        protected override void OnDestroy()
        {
            RemoveBodyEntity();

            base.OnDestroy();
        }
    }

}
