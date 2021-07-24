using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player.Camera;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public PlayerCorpse PlayerCorpse { get; set; }

        [Net]
        public int CorpseCredits { get; set; } = 0;

        public bool IsConfirmed = false;

        public bool IsMissingInAction = false;

        public TTTPlayer CorpseConfirmer = null;

        private const float INSPECT_CORPSE_DISTANCE = 80f;

        private PlayerCorpse _inspectingPlayerCorpse = null;

        public void RemovePlayerCorpse()
        {
            if (PlayerCorpse == null || !PlayerCorpse.IsValid())
            {
                return;
            }

            PlayerCorpse.Delete();
            PlayerCorpse = null;
        }

        private void TickAttemptInspectPlayerCorpse()
        {
            using (Prediction.Off())
            {
                PlayerCorpse playerCorpse = IsLookingAtPlayerCorpse();

                if (playerCorpse != null)
                {
                    if (IsServer && !playerCorpse.IsIdentified && Input.Down(InputButton.Use) && LifeState == LifeState.Alive)
                    {
                        playerCorpse.IsIdentified = true;

                        // TODO Handling if a player disconnects!
                        if (playerCorpse.Player != null && playerCorpse.Player.IsValid())
                        {
                            playerCorpse.Player.IsConfirmed = true;
                            playerCorpse.Player.CorpseConfirmer = this;

                            int credits = playerCorpse.Player.Credits;

                            if (credits > 0)
                            {
                                Credits += credits;
                                playerCorpse.Player.Credits = 0;
                                playerCorpse.Player.CorpseCredits = credits;
                            }

                            playerCorpse.Player.GetClientOwner()?.SetScore("alive", false);

                            RPCs.ClientConfirmPlayer(this, playerCorpse, playerCorpse.Player, playerCorpse.Player.Role.Name);
                        }
                    }

                    if (_inspectingPlayerCorpse != playerCorpse)
                    {
                        _inspectingPlayerCorpse = playerCorpse;

                        if (IsClient)
                        {
                            InspectMenu.Instance.InspectCorpse(playerCorpse.Player);
                        }
                    }
                }
                else if (_inspectingPlayerCorpse != null)
                {
                    if (IsClient && InspectMenu.Instance.IsShowing)
                    {
                        InspectMenu.Instance.IsShowing = false;
                    }

                    _inspectingPlayerCorpse = null;
                }
            }
        }

        private PlayerCorpse IsLookingAtPlayerCorpse()
        {
            // TODO ignore dead players as well, they could block this
            TraceResult trace;

            if (Camera is FreeSpectateCamera camera)
            {
                trace = Trace.Ray(camera.Pos, camera.Pos + camera.Rot.Forward * INSPECT_CORPSE_DISTANCE)
                    .HitLayer(CollisionLayer.Debris)
                    .Ignore(this)
                    .Run();
            }
            else
            {
                Trace tr = Trace.Ray(EyePos, EyePos + EyeRot.Forward * INSPECT_CORPSE_DISTANCE)
                    .HitLayer(CollisionLayer.Debris)
                    .Ignore(this);

                if (IsSpectatingPlayer)
                {
                    tr.Ignore(CurrentPlayer);
                }

                trace = tr.Run();
            }

            if (trace.Hit && trace.Entity is PlayerCorpse corpse)
            {
                return corpse;
            }

            return null;
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
