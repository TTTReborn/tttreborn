// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

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
