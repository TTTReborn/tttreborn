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

using System;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("entity_weapon_random")]
    public class TTTWeaponRandom : Entity
    {
        private static readonly int AMMO_DISTANCE_UP = 24;

        /// <summary>
        /// Defines the amount of matching ammo entities that should be spawned near the weapons.
        /// </summary>
        [Property(Title = "Amount of Ammo")]
        public int AmmoToSpawn { get; set; } = 0;

        public void Activate()
        {
            List<Type> wepTypes = Globals.Utils.GetTypesWithAttribute<TTTWeapon, SpawnableAttribute>();

            if (wepTypes.Count <= 0)
            {
                return;
            }

            Type weaponTypeToSpawn = Utils.RNG.FromList(wepTypes);
            TTTWeapon weapon = Globals.Utils.GetObjectByType<TTTWeapon>(weaponTypeToSpawn);
            weapon.Position = Position;
            weapon.Rotation = Rotation;
            weapon.Spawn();

            if (weapon.AmmoEntity == null)
            {
                return; // If the choosen weapon doesn't use ammo we don't need to spawn any.
            }

            if (!weapon.AmmoEntity.IsSubclassOf(typeof(TTTAmmo)))
            {
                Log.Error($"The defined ammo type {weapon.AmmoEntity.Name} for the weapon {weapon.LibraryName} is not a descendant of {typeof(TTTAmmo).Name}.");

                return;
            }

            for (int i = 0; i < AmmoToSpawn; i++)
            {
                TTTAmmo ammo = Utils.GetObjectByType<TTTAmmo>(weapon.AmmoEntity);
                ammo.Position = weapon.Position + Vector3.Up * AMMO_DISTANCE_UP;
                ammo.Spawn();
            }
        }
    }
}
