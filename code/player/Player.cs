using Sandbox;

using TTTReborn.Gamemode;
using TTTReborn.Player.Camera;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : Sandbox.Player
    {
        public enum RoleType { None, Innocent, Detective, Traitor }
        public PlayerCorpse PlayerCorpse { get; set; }

        [Net, Local]
        public RoleType Role { get; set; }

        [Net, Local]
        public int Credits { get; set; } = 0;

        private DamageInfo _lastDamageInfo;

        public TTTPlayer()
        {

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
            SetModel("models/citizen/citizen.vmdl");

            Controller = new WalkController();
            Animator = new StandardPlayerAnimator();
            Camera = new FirstPersonCamera();
            Inventory = new Inventory(this);

            EnableAllCollisions = true;
            EnableDrawing = true;
            EnableHideInFirstPerson = true;
            EnableShadowInFirstPerson = true;

            Role = RoleType.None;
            Credits = 0;

            base.Respawn();
        }

        public override void OnKilled()
        {
            base.OnKilled();

            BecomePlayerCorpseOnServer(_lastDamageInfo.Force, GetHitboxBone(_lastDamageInfo.HitboxIndex));
            Inventory.DeleteContents();
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
            /*
            if (_timeSinceDropped < 1)
            {
                return;
            }
            */

            base.StartTouch(other);
        }

        private void TickInspectPlayerCorpse()
        {
            // TODO: Handle role specific corpse inspecting.
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

            if (trace.Hit && trace.Entity is PlayerCorpse corpse && corpse.Player != null)
            {
                InspectPlayerCorpse(corpse);
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

        public void RemovePlayerCorpse()
        {
            if (PlayerCorpse != null && PlayerCorpse.IsValid())
            {
                PlayerCorpse.Delete();
                PlayerCorpse = null;
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
        public void InspectPlayerCorpse(PlayerCorpse corpse)
        {

        }

        protected override void OnDestroy()
        {
            RemovePlayerCorpse();

            base.OnDestroy();
        }

        private void BecomePlayerCorpseOnServer(Vector3 force, int forceBone)
        {
            PlayerCorpse corpse = new PlayerCorpse
            {
                Position = Position,
                Rotation = Rotation
            };

            corpse.CopyFrom(this);
            corpse.ApplyForceToBone(force, forceBone);
            corpse.Player = this;

            PlayerCorpse = corpse;
        }
    }

}
