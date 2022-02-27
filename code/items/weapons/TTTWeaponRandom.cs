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
        [Property(Title = "Amount of ammo")]
        public int AmmoToSpawn { get; set; } = 0;

        /// <summary>
        /// Spawn weapons of a specific category.
        /// </summary>
        [Property(Title = "Spawn weapons of a specific category")]
        public CarriableCategories? Category { get; set; } = null;

        public virtual void Activate()
        {
            List<Type> wepTypes = Utils.GetTypesWithAttribute<TTTWeapon, SpawnableAttribute>();

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
            TTTWeapon weapon = Utils.GetObjectByType<TTTWeapon>(weaponTypeToSpawn);
            weapon.Position = Position;
            weapon.Rotation = Rotation;
            weapon.Spawn();

            if (weapon.AmmoType == null)
            {
                return; // If the choosen weapon doesn't use ammo we don't need to spawn any.
            }

            if (!weapon.AmmoType.IsSubclassOf(typeof(TTTAmmo)))
            {
                Log.Error($"The defined ammo type {weapon.AmmoType.Name} for the weapon {weapon.LibraryName} is not a descendant of {typeof(TTTAmmo).Name}.");

                return;
            }

            for (int i = 0; i < AmmoToSpawn; i++)
            {
                TTTAmmo ammo = Utils.GetObjectByType<TTTAmmo>(weapon.AmmoType);
                ammo.Position = weapon.Position + Vector3.Up * AMMO_DISTANCE_UP;
                ammo.Spawn();
            }
        }
    }
}
