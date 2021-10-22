using Sandbox;

using TTTReborn.Globals;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public struct ConfirmationData
    {
        public bool Identified;
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
                if (IsClient && !Input.Down(InputButton.Use))
                {
                    InspectMenu.Instance.Enabled = false;

                    return;
                }

                PlayerCorpse playerCorpse = IsLookingAtType<PlayerCorpse>(INSPECT_CORPSE_DISTANCE);
                if (playerCorpse == null)
                {
                    return;
                }

                if (IsServer && !playerCorpse.IsIdentified && LifeState == LifeState.Alive && Input.Down(InputButton.Use))
                {
                    playerCorpse.IsIdentified = true;

                    // TODO: Handle player disconnects.
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

                        RPCs.ClientConfirmPlayer(this, playerCorpse, playerCorpse.Player, playerCorpse.Player.Role.Name, playerCorpse.Player.Team.Name, playerCorpse.GetConfirmationData(), playerCorpse.KillerWeapon, playerCorpse.Perks);
                    }
                }

                if (Input.Down(InputButton.Use) && playerCorpse.IsIdentified)
                {
                    ClientEnableInspectMenu(playerCorpse);
                }
            }
        }

        public static void ClientEnableInspectMenu(PlayerCorpse playerCorpse)
        {
            if (InspectMenu.Instance != null && !InspectMenu.Instance.Enabled)
            {
                InspectMenu.Instance.InspectCorpse(playerCorpse);
            }
        }

        private void BecomePlayerCorpseOnServer(Vector3 force, int forceBone)
        {
            PlayerCorpse corpse = new()
            {
                Position = Position,
                Rotation = Rotation
            };

            corpse.KillerWeapon = LastDamageWeapon?.LibraryName;
            corpse.WasHeadshot = LastDamageWasHeadshot;
            corpse.Distance = LastDistanceToAttacker;
            corpse.Suicide = LastAttacker == this;

            PerksInventory perksInventory = Inventory.Perks;

            corpse.Perks = new string[perksInventory.Count()];

            for (int i = 0; i < corpse.Perks.Length; i++)
            {
                corpse.Perks[i] = perksInventory.Get(i).LibraryName;
            }

            corpse.CopyFrom(this);
            corpse.ApplyForceToBone(force, forceBone);

            PlayerCorpse = corpse;
        }
    }
}
