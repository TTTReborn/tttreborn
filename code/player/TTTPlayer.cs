using Sandbox;

using TTTReborn.Globals;
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

        private DamageInfo _lastDamageInfo;

        private TimeSince _timeSinceDropped = 0;

        public TTTPlayer()
        {
            Inventory = new Inventory(this);

            _lastGroundEntity = GroundEntity;
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

            ShowFlashlight(false, false);
        }

        // Important: Server-side only
        public void InitialRespawn()
        {
            Respawn();

            bool isPostRound = Gamemode.Game.Instance.Round is Rounds.PostRound;

            // sync roles
            using (Prediction.Off())
            {
                foreach (TTTPlayer player in Utils.GetPlayers())
                {
                    if (isPostRound || player.IsConfirmed)
                    {
                        RPCs.ClientSetRole(To.Single(this), player, player.Role.Name);
                    }
                }
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

            Credits = 0;

            SetRole(new NoneRole());

            GetClientOwner().SetScore("alive", true);

            IsMissingInAction = false;

            using (Prediction.Off())
            {
                RPCs.ClientOnPlayerSpawned(this);
                RPCs.ClientSetRole(To.Single(this), this, Role.Name);
            }

            RemovePlayerCorpse();
            Inventory.DeleteContents();
            Gamemode.Game.Instance.Round.OnPlayerSpawn(this);

            base.Respawn();

            switch (Gamemode.Game.Instance.Round)
            {
                // hacky
                // TODO use a spectator flag, otherwise, no player can respawn during round with an item etc.
                // TODO spawn player as spectator instantly
                case Rounds.PreRound:
                    IsConfirmed = false;
                    CorpseConfirmer = null;

                    break;
            }
        }

        public override void OnKilled()
        {
            base.OnKilled();

            BecomePlayerCorpseOnServer(_lastDamageInfo.Force, GetHitboxBone(_lastDamageInfo.HitboxIndex));

            Inventory.DropActive();
            Inventory.DeleteContents();

            ShowFlashlight(false, false);

            IsMissingInAction = true;

            using (Prediction.Off())
            {
                RPCs.ClientOnPlayerDied(To.Single(this), this);
                SyncMIA();
            }
        }

        public override void Simulate(Client client)
        {
            // Input requested a carriable entity switch
            if (Input.ActiveChild != null)
            {
                ActiveChild = Input.ActiveChild;
            }

            if (LifeState != LifeState.Alive)
            {
                return;
            }

            TickItemSimulate();
            TickPlayerUse();
            TickPlayerDropCarriable();

            SimulateActiveChild(client, ActiveChild);

            if (IsServer)
            {
                TickAttemptInspectPlayerCorpse();
                TickPlayerFalling();
            }

            PawnController controller = GetActiveController();
            controller?.Simulate(client, this, GetActiveAnimator());

            TickPlayerFlashlight();
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

        private void TickPlayerDropCarriable()
        {
            if (Input.Pressed(InputButton.Drop) && ActiveChild != null && Inventory != null)
            {
                Entity droppedEntity = Inventory.DropActive();

                if (droppedEntity != null)
                {
                    if (droppedEntity.PhysicsGroup != null)
                    {
                        droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRot.Forward + EyeRot.Up) * CarriableDropVelocity;
                    }

                    _timeSinceDropped = 0;
                }
            }
        }

        private void TickItemSimulate()
        {
            Client client = GetClientOwner();

            if (client == null)
            {
                return;
            }

            PerksInventory perks = (Inventory as Inventory).Perks;

            for (int i = 0; i < perks.Count(); i++)
            {
                perks.Get(i).Simulate(client);
            }
        }

        protected override void OnDestroy()
        {
            RemovePlayerCorpse();

            base.OnDestroy();
        }
    }
}
