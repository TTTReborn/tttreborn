using System.Linq;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Items;
using TTTReborn.Player.Camera;
using TTTReborn.Roles;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : Sandbox.Player
    {
        private static int CarriableDropVelocity { get; set; } = 300;

        [Net, Local]
        public int Credits { get; set; } = 0;

        [Net]
        public bool IsForcedSpectator { get; set; } = false;

        public bool IsInitialSpawning { get; set; } = false;

        public new Inventory Inventory
        {
            get => (Inventory) base.Inventory;
            private init => base.Inventory = value;
        }

        public new DefaultWalkController Controller
        {
            get => (DefaultWalkController) base.Controller;
            private set => base.Controller = value;
        }

        private DamageInfo _lastDamageInfo;

        public TTTPlayer()
        {
            Inventory = new Inventory(this);
        }

        // Important: Server-side only
        public void InitialSpawn()
        {
            bool isPostRound = Gamemode.Game.Instance.Round is Rounds.PostRound;

            IsInitialSpawning = true;
            IsForcedSpectator = isPostRound || Gamemode.Game.Instance.Round is Rounds.InProgressRound;

            Respawn();

            // sync roles
            using (Prediction.Off())
            {
                foreach (TTTPlayer player in Utils.GetPlayers())
                {
                    if (isPostRound || player.IsConfirmed)
                    {
                        player.SendClientRole(To.Single(this));
                    }
                }

                Client.SetValue("forcedspectator", IsForcedSpectator);

                Event.Run(TTTEvent.Player.INITIAL_SPAWN, Client);

                ClientInitialSpawn();
            }

            IsInitialSpawning = false;
            IsForcedSpectator = false;
        }

        // Important: Server-side only
        // TODO: Convert to a player.RPC, event based system found inside of...
        // TODO: https://github.com/TTTReborn/ttt-reborn/commit/1776803a4b26d6614eba13b363bbc8a4a4c14a2e#diff-d451f87d88459b7f181b1aa4bbd7846a4202c5650bd699912b88ff2906cacf37R30
        public override void Respawn()
        {
            SetModel("models/citizen/citizen.vmdl");

            Animator = new StandardPlayerAnimator();

            EnableHideInFirstPerson = true;
            EnableShadowInFirstPerson = false;
            EnableDrawing = true;

            Credits = 0;

            SetRole(new NoneRole());

            IsMissingInAction = false;

            using (Prediction.Off())
            {
                Event.Run(TTTEvent.Player.SPAWNED, this);

                RPCs.ClientOnPlayerSpawned(this);
                SendClientRole();
            }

            base.Respawn();

            if (!IsForcedSpectator)
            {
                Controller = new DefaultWalkController();
                CameraMode = new FirstPersonCamera();

                EnableAllCollisions = true;
                EnableDrawing = true;
            }
            else
            {
                MakeSpectator(false);
            }

            RemovePlayerCorpse();
            Inventory.DeleteContents();
            Gamemode.Game.Instance.Round.OnPlayerSpawn(this);

            switch (Gamemode.Game.Instance.Round)
            {
                // hacky
                // TODO use a spectator flag, otherwise, no player can respawn during round with an item etc.
                // TODO spawn player as spectator instantly
                case Rounds.PreRound:
                    IsConfirmed = false;
                    CorpseConfirmer = null;

                    Client.SetValue("forcedspectator", false);

                    break;
            }
        }

        public override void OnKilled()
        {
            base.OnKilled();

            BecomePlayerCorpseOnServer(_lastDamageInfo.Force, GetHitboxBone(_lastDamageInfo.HitboxIndex));

            Inventory.DropAll();
            Inventory.DeleteContents();

            ShowFlashlight(false, false);

            IsMissingInAction = true;

            using (Prediction.Off())
            {
                RPCs.ClientOnPlayerDied(this);

                if (Gamemode.Game.Instance.Round is Rounds.InProgressRound)
                {
                    SyncMIA();
                }
                else if (Gamemode.Game.Instance.Round is Rounds.PostRound && PlayerCorpse != null && !PlayerCorpse.IsIdentified)
                {
                    PlayerCorpse.IsIdentified = true;

                    RPCs.ClientConfirmPlayer(null, PlayerCorpse, this, Role.Name, Team.Name, PlayerCorpse.GetConfirmationData(), PlayerCorpse.KillerWeapon, PlayerCorpse.Perks);
                }
            }
        }

        public override void Simulate(Client client)
        {
            if (IsClient)
            {
                TickPlayerVoiceChat();
            }
            else
            {
                TickAFKSystem();
            }

            TickEntityHints();

            if (LifeState != LifeState.Alive)
            {
                TickPlayerChangeSpectateCamera();

                return;
            }

            // Input requested a carriable entity switch
            if (Input.ActiveChild != null)
            {
                ActiveChild = Input.ActiveChild;
            }

            SimulateActiveChild(client, ActiveChild);

            TickC4Simulate();
            TickItemSimulate();
            TickPlayerUse();
            TickPlayerDropCarriable();
            TickPlayerFlashlight();
            TickPlayerShop();
            TickLogicButtonActivate();

            PawnController controller = GetActiveController();
            controller?.Simulate(client, this, GetActiveAnimator());
        }

        protected override void UseFail()
        {
            // Do nothing. By default this plays a sound that we don't want.
        }

        public override void StartTouch(Entity other)
        {
            if (IsClient)
            {
                return;
            }

            if (other is PickupTrigger)
            {
                StartTouch(other.Parent);
            }
        }

        private void TickPlayerDropCarriable()
        {
            if (Input.Pressed(InputButton.Drop) && !Input.Down(InputButton.Run) && ActiveChild != null && Inventory != null)
            {
                Entity droppedEntity = Inventory.DropActive();

                if (droppedEntity != null)
                {
                    if (droppedEntity.PhysicsGroup != null)
                    {
                        droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRotation.Forward + EyeRotation.Up) * CarriableDropVelocity;
                    }
                }
            }
        }

        private void TickPlayerChangeSpectateCamera()
        {
            if (!Input.Pressed(InputButton.Jump) || !IsServer)
            {
                return;
            }

            using (Prediction.Off())
            {
                CameraMode = CameraMode switch
                {
                    RagdollSpectateCamera => new FreeSpectateCamera(),
                    FreeSpectateCamera => new ThirdPersonSpectateCamera(),
                    ThirdPersonSpectateCamera => new FirstPersonSpectatorCamera(),
                    FirstPersonSpectatorCamera => new FreeSpectateCamera(),
                    _ => CameraMode
                };
            }
        }

        private void TickItemSimulate()
        {
            if (Client == null)
            {
                return;
            }

            PerksInventory perks = Inventory.Perks;

            for (int i = 0; i < perks.Count(); i++)
            {
                perks.Get(i).Simulate(Client);
            }
        }

        private void TickC4Simulate()
        {
            foreach (C4Entity c4 in All.Where(x => x is C4Entity))
            {
                c4.Simulate(Client);
            }
        }

        protected override void OnDestroy()
        {
            RemovePlayerCorpse();

            base.OnDestroy();
        }
    }
}
