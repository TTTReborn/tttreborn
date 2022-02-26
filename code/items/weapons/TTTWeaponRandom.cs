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
