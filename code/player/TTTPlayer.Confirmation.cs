using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.UI;

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
                PlayerCorpse playerCorpse = IsLookingAtType<PlayerCorpse>(INSPECT_CORPSE_DISTANCE);

                if (playerCorpse != null)
                {
                    if (IsServer && !playerCorpse.IsIdentified && Input.Pressed(InputButton.Use) && LifeState == LifeState.Alive)
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

                            RPCs.ClientConfirmPlayer(this, playerCorpse, playerCorpse.Player, playerCorpse.Player.Role.Name, playerCorpse.Player.Team.Name, playerCorpse.GetConfirmationData(), playerCorpse.KillerWeapon, playerCorpse.Perks);
                        }
                    }

                    if (_inspectingPlayerCorpse != playerCorpse)
                    {
                        _inspectingPlayerCorpse = playerCorpse;

                        if (IsClient)
                        {
                            InspectMenu.Instance.InspectCorpse(playerCorpse.Player, playerCorpse.IsIdentified, playerCorpse.GetConfirmationData(), playerCorpse.KillerWeapon, playerCorpse.Perks);
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

        private void BecomePlayerCorpseOnServer(Vector3 force, int forceBone)
        {
            PlayerCorpse corpse = new PlayerCorpse
            {
                Position = Position,
                Rotation = Rotation
            };

            corpse.KillerWeapon = LastDamageWeapon?.Name;
            corpse.WasHeadshot = LastDamageWasHeadshot;
            corpse.Distance = LastDistanceToAttacker;
            corpse.Suicide = LastAttacker == this;

            List<string> perks = new();
            PerksInventory perksInventory = (Inventory as Inventory).Perks;

            for (int i = 0; i < perksInventory.Count(); i++)
            {
                perks.Add(perksInventory.Get(i).Name);
            }

            corpse.Perks = perks.ToArray();

            corpse.CopyFrom(this);
            corpse.ApplyForceToBone(force, forceBone);

            PlayerCorpse = corpse;
        }
    }
}
