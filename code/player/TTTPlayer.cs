using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

using TTTReborn.Gamemode;
using TTTReborn.Player.Camera;
using TTTReborn.Roles;
using TTTReborn.UI;
using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : Sandbox.Player
    {
        public PlayerCorpse PlayerCorpse { get; set; }

        private static int WeaponDropVelocity { get; set; } = 300;

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

        private TimeSince timeSinceDropped = 0;

        public TTTPlayer()
        {
            Inventory = new Inventory(this);
        }

        public static List<TTTPlayer> GetAll()
        {
            List<TTTPlayer> playerList = new();

            foreach (Entity entity in All)
            {
                if (entity is TTTPlayer player)
                {
                    playerList.Add(player);
                }
            }

            return playerList;
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

        // Important: Server-side only
        // TODO: Convert to a player.RPC, event based system found inside of...
        // TODO: https://github.com/TTTReborn/ttt-reborn/commit/1776803a4b26d6614eba13b363bbc8a4a4c14a2e#diff-d451f87d88459b7f181b1aa4bbd7846a4202c5650bd699912b88ff2906cacf37R30
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

            Role = new NoneRole();
            Credits = 0;

            using(Prediction.Off())
            {
                ClientOnPlayerSpawned(To.Single(this));
            }

            RemovePlayerCorpse();
            Inventory.DeleteContents();
            TTTReborn.Gamemode.Game.Instance?.Round?.OnPlayerSpawn(this);
            base.Respawn();
        }

        public override void OnKilled()
        {
            base.OnKilled();

            BecomePlayerCorpseOnServer(lastDamageInfo.Force, GetHitboxBone(lastDamageInfo.HitboxIndex));

            Inventory.DropActive();
            Inventory.DeleteContents();

            using(Prediction.Off())
            {
                ClientOnPlayerDied(To.Single(this));
            }
        }

        public override void Simulate(Client client)
        {
            // Input requested a weapon switch
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
            TickPlayerDropWeapon();

            SimulateActiveChild(client, ActiveChild);

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
            if (timeSinceDropped < 1)
            {
                return;
            }

            base.StartTouch(other);
        }

        private void TickPlayerDropWeapon()
        {
            if (Input.Pressed(InputButton.Drop) && ActiveChild != null && Inventory != null)
            {
                int weaponSlot = (int) (ActiveChild as TTTWeapon).WeaponType;
                Entity droppedEntity = Inventory.DropActive();

                if (droppedEntity != null)
                {
                    if (droppedEntity.PhysicsGroup != null)
                    {
                        droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRot.Forward + EyeRot.Up) * WeaponDropVelocity;
                    }

                    timeSinceDropped = 0;
                }
            }
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

                            ClientConfirmPlayer(this, playerCorpse.Player, playerCorpse.Player.Role.Name);
                        }

                        // Send the request to the player looking at the player corpse.
                        // https://wiki.facepunch.com/sbox/RPCs#targetingplayers
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
                attacker.ClientDidDamage(info.Position, info.Damage, ((float)Health).LerpInverse(100, 0));
            }

            if (info.Weapon != null)
            {
                ClientTookDamage(info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position);
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

        public void RequestPurchase(IBuyableItem item)
        {
            if (item.IsBuyable(this))
            {
                item.Equip(this);

                return;
            }

            Log.Warning($"{GetClientOwner().Name} tried to buy '{item.GetName()}'.");
        }
    }
}
