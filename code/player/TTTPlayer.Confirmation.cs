using Sandbox;

using TTTReborn.Globals;

namespace TTTReborn.Player
{
    public struct ConfirmationData
    {
        public bool Headshot;
        public bool Suicide;
        public float Time;
        public float Distance;
        // TODO damage type
    }

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
                To client = To.Single(this);
                PlayerCorpse playerCorpse = IsLookingAtPlayerCorpse();

                if (playerCorpse != null)
                {
                    if (_inspectingPlayerCorpse != playerCorpse)
                    {
                        _inspectingPlayerCorpse = playerCorpse;

                        // Send the request to the player looking at the player corpse.
                        // https://wiki.facepunch.com/sbox/RPCs#targetingplayers
                        ClientOpenInspectMenu(client, playerCorpse.Player, playerCorpse.IsIdentified, GetConfirmationData(playerCorpse), playerCorpse.KillerWeapon?.Name);
                    }

                    if (!playerCorpse.IsIdentified && Input.Down(InputButton.Use))
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

                            RPCs.ClientConfirmPlayer(this, playerCorpse.Player, playerCorpse.Player.Role.Name);

                            ClientOpenInspectMenu(client, playerCorpse.Player, playerCorpse.IsIdentified, GetConfirmationData(playerCorpse), playerCorpse.KillerWeapon?.Name);
                        }
                    }

                    return;
                }

                if (_inspectingPlayerCorpse != null)
                {
                    ClientCloseInspectMenu(client);

                    _inspectingPlayerCorpse = null;
                }
            }
        }

        private ConfirmationData GetConfirmationData(PlayerCorpse playerCorpse)
        {
            return new ConfirmationData
            {
                Headshot = playerCorpse.WasHeadshot,
                Time = playerCorpse.KilledTime,
                Distance = playerCorpse.Distance,
                Suicide = playerCorpse.Suicide
            };
        }

        private PlayerCorpse IsLookingAtPlayerCorpse()
        {
            TraceResult trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * INSPECT_CORPSE_DISTANCE)
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

        private void BecomePlayerCorpseOnServer(Vector3 force, int forceBone)
        {
            PlayerCorpse corpse = new PlayerCorpse
            {
                Position = Position,
                Rotation = Rotation
            };

            corpse.KillerWeapon = LastDamageWeapon;
            corpse.WasHeadshot = LastDamageWasHeadshot;
            corpse.Distance = LastDistanceToAttacker;
            corpse.Suicide = LastAttacker == this;

            corpse.CopyFrom(this);
            corpse.ApplyForceToBone(force, forceBone);
            corpse.Player = this;

            PlayerCorpse = corpse;
        }
    }
}
