using Sandbox;

using TTTReborn.Gamemode;
using TTTReborn.Player.Camera;
using TTTReborn.Roles;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : Sandbox.Player
    {
        public PlayerCorpse PlayerCorpse { get; set; }

        public BaseRole Role {
            set
            {
                role = value;

                if (IsServer)
                {
                    ClientSetRole(To.Single(this), role.Name);
                }
            }
            get {
                return role;
            }
        }

        private BaseRole role = new NoneRole();

        [Net, Local]
        public int Credits { get; set; } = 0;

        private DamageInfo lastDamageInfo;
        private float inspectCorpseDistance = 80f;

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

        public void RemovePlayerCorpse()
        {
            if (PlayerCorpse != null && PlayerCorpse.IsValid())
            {
                PlayerCorpse.Delete();
                PlayerCorpse = null;
            }
        }

        // TODO: Convert to a player.RPC, event based system found inside of...
        // TODO: https://github.com/TTTReborn/ttt-reborn/commit/1776803a4b26d6614eba13b363bbc8a4a4c14a2e#diff-d451f87d88459b7f181b1aa4bbd7846a4202c5650bd699912b88ff2906cacf37R30
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

            Role = new NoneRole();
            Credits = 0;

            RemovePlayerCorpse();
            TTTReborn.Gamemode.Game.Instance?.Round?.OnPlayerSpawn(this);
            base.Respawn();
        }

        public override void OnKilled()
        {
            base.OnKilled();

            BecomePlayerCorpseOnServer(lastDamageInfo.Force, GetHitboxBone(lastDamageInfo.HitboxIndex));
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
            TickAttemptInspectPlayerCorpse();

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

        private void TickAttemptInspectPlayerCorpse()
        {
            if (IsServer)
            {
                using (Prediction.Off())
                {
                    To client = To.Single(this);
                    PlayerCorpse playerCorpse = IsLookingAtPlayerCorpse();

                    if (playerCorpse != null)
                    {
                        if (Input.Down(InputButton.Use) && !playerCorpse.IsIdentified)
                        {
                            playerCorpse.IsIdentified = true;
                            Client playerCorpseInfo = playerCorpse.Player.GetClientOwner();

                            ClientDisplayIdentifiedMessage(this.Controller.Client.SteamId,
                                this.Controller.Client.Name,
                                playerCorpseInfo.SteamId,
                                playerCorpseInfo.Name,
                                playerCorpse.Player.Role.Name
                            );
                        }

                        // Send the request to the player looking at the player corpse.
                        // https://wiki.facepunch.com/sbox/RPCs#targetingplayers
                        // TODO: Figure out why directly passing in just playerCorpse to "ClientOpenInspectMenu" makes playerCorpse.Player null.
                        ClientOpenInspectMenu(client, playerCorpse.Player, playerCorpse.IsIdentified);

                        return;
                    }

                    ClientCloseInspectMenu(client);
                }
            }
        }

        private PlayerCorpse IsLookingAtPlayerCorpse()
        {
            TraceResult trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * inspectCorpseDistance)
                .HitLayer(CollisionLayer.Debris)
                .Ignore(ActiveChild)
                .Ignore(this)
                .Radius(2)
                .Run();

            if (trace.Hit && trace.Entity is PlayerCorpse corpse && corpse.Player != null)
            {
                return corpse;
            }

            return null;
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

            lastDamageInfo = info;

            base.TakeDamage(info);
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
