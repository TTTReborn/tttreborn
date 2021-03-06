using System;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_random")]
    public class WeaponRandom : Entity
    {
        private static readonly int AMMO_DISTANCE_UP = 24;

        /// <summary>
        /// Defines the amount of matching ammo entities that should be spawned near the weapons.
        /// </summary>
        [Property(Title = "Amount of ammo")]
        public int AmmoToSpawn { get; set; } = 0;

        /// <summary>
        /// Spawn weapons of a specific category.
        /// </summary>
        [Property(Title = "Spawn weapons of a specific category")]
        public CarriableCategories? Category { get; set; } = null;

        public virtual void Activate()
        {
            List<Type> wepTypes = Utils.GetTypesWithAttribute<Weapon, SpawnableAttribute>();

            if (Category != null)
            {
                List<Type> filteredTypes = new();

                foreach (Type type in wepTypes)
                {
                    WeaponAttribute weaponAttribute = Utils.GetAttribute<WeaponAttribute>(type);

                    if (weaponAttribute != null && weaponAttribute.Category == Category)
                    {
                        filteredTypes.Add(type);
                    }
                }

                wepTypes = filteredTypes;
            }

            if (wepTypes.Count <= 0)
            {
                return;
            }

            Type weaponTypeToSpawn = Utils.RNG.FromList(wepTypes);
            Weapon weapon = Utils.GetObjectByType<Weapon>(weaponTypeToSpawn);

            if (weapon == null || !weapon.IsValid)
            {
                Log.Debug($"Failed to initialize random weapon of type '{weaponTypeToSpawn}': {weapon}");

                return;
            }

            weapon.Position = Position;
            weapon.Rotation = Rotation;
            weapon.Spawn();

            string ammoName = weapon.Primary.AmmoName;

            if (ammoName == null)
            {
                return; // If the choosen weapon doesn't use ammo we don't need to spawn any
            }

            Type ammoType = Utils.GetTypeByLibraryName<Ammo>(ammoName);

            if (ammoType == null)
            {
                return; // If the choosen weapon doesn't use ammo we don't need to spawn any
            }

            if (!ammoType.IsSubclassOf(typeof(Ammo)))
            {
                Log.Error($"The defined ammo type {ammoType.Name} for the weapon {weapon.Info.LibraryName} is not a descendant of {typeof(Ammo).Name}.");

                return;
            }

            for (int i = 0; i < AmmoToSpawn; i++)
            {
                Ammo ammo = Utils.GetObjectByType<Ammo>(ammoType);
                ammo.Position = weapon.Position + Vector3.Up * AMMO_DISTANCE_UP;
                ammo.Spawn();
            }
        }
    }
}
