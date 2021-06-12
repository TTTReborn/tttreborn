using Sandbox;
using Sandbox.UI;

using TTTReborn.Player.Camera;
using TTTReborn.UI;
using TTTReborn.Weapons;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : Sandbox.Player
    {
        public enum RoleType { None, Innocent, Detective, Traitor }
        public PlayerCorpse PlayerCorpse { get; set; }

        // TODO: Make LOCAL, if it isn't this data gets networked to all players, could cause hackers.
        // TODO: It currently isn't because all Networked information needs to be transfered to player corpse for inspecting bodies.
        [Net]
        public RoleType Role { get; set; }

        [Net, Local]
        public int Credits { get; set; } = 0;

        private DamageInfo _lastDamageInfo;
        private float _inspectCorpseDistance = 80f;

        private TimeSince timeSinceDropped = 0;

        public TTTPlayer()
        {
            Inventory = new Inventory(this);
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

            Role = RoleType.None;
            Credits = 0;

            RemovePlayerCorpse();
            Inventory.DeleteContents();
            TTTReborn.Gamemode.Game.Instance?.Round?.OnPlayerSpawn(this);
            base.Respawn();
        }

        public override void OnKilled()
        {
            base.OnKilled();

            BecomePlayerCorpseOnServer(_lastDamageInfo.Force, GetHitboxBone(_lastDamageInfo.HitboxIndex));

            Inventory.DropActive();
            Inventory.DeleteContents();
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

            if (Input.Pressed(InputButton.Drop) && ActiveChild != null && Inventory != null)
            {
                int weaponSlot = (int) (ActiveChild as Weapon).WeaponType;
                Entity droppedEntity = Inventory.DropActive();

                if (droppedEntity != null)
                {
                    if (droppedEntity.PhysicsGroup != null)
                    {
                        droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRot.Forward + EyeRot.Up) * 300;
                    }

                    timeSinceDropped = 0;
                }
            }

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

        private void TickAttemptInspectPlayerCorpse()
        {
            if (IsClient)
            {
                if (InspectMenu.Instance?.IsShowing ?? false)
                {
                    // Menu is showing already, bail out.
                    return;
                }
            }

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
                                playerCorpse.Player.Role.ToString()
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
            TraceResult trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * _inspectCorpseDistance)
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

            _lastDamageInfo = info;

            base.TakeDamage(info);
        }

        [ClientRpc]
        public static void ClientOpenInspectMenu(TTTPlayer deadPlayer, bool isIdentified)
        {
            InspectMenu.Instance?.InspectCorpse(deadPlayer, isIdentified);
        }

        [ClientRpc]
        public static void ClientCloseInspectMenu()
        {
            if (InspectMenu.Instance?.IsShowing ?? false)
            {
                InspectMenu.Instance.IsShowing = false;
            }
        }

        [ClientRpc]
        public static void ClientDisplayIdentifiedMessage(ulong leftId, string leftName, ulong rightId, string rightName, string role)
        {
            // TODO: Refactor the UI element, and provide a better interface for passing in these parameters.
            InfoFeed.Current?.AddEntry(leftId, leftName, rightId, $"{rightName}. Their role was {role}!", "found the body of");
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
