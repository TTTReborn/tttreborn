using Sandbox;

using TTTReborn.UI;

namespace TTTReborn
{
    public partial class Player
    {
        public PlayerCorpse PlayerCorpse { get; set; }

        public bool IsConfirmed = false;

        public bool IsMissingInAction = false;

        public Player CorpseConfirmer = null;

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
            PlayerCorpse = new()
            {
                Position = Position,
                Rotation = Rotation
            };

            PlayerCorpse.CopyFrom(this);
            PlayerCorpse.ApplyForceToBone(force, forceBone);

            Credits = 0;
        }
    }
}
