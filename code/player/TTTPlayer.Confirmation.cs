using Sandbox;

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

        public void RemovePlayerCorpse()
        {
            if (PlayerCorpse == null || !PlayerCorpse.IsValid())
            {
                return;
            }

            PlayerCorpse.Delete();
            PlayerCorpse = null;
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
