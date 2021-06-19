using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTTReborn.Items
{
    public static class WeaponFunctions
    {
        /// <summary>
        /// Loops through every type derived from `TTTReborn.Weapons.Weapon` and collects non-abstract weapons.
        /// </summary>
        /// <returns>List of all available weapons</returns>
        public static List<Type> GetWeapons()
        {
            List<Type> weaponTypes = new();

            Library.GetAll<TTTWeapon>().ToList().ForEach(t =>
            {
                if (!t.IsAbstract && !t.ContainsGenericParameters)
                {
                    weaponTypes.Add(t);
                }
            });

            return weaponTypes;
        }

        /// <summary>
        /// Get a `Type` of `TTTReborn.Weapons.Weapon` by it's name (`TTTReborn.Weapons.WeaponAttribute`).
        /// </summary>
        /// <param name="weaponName">The name of the `TTTReborn.Weapons.WeaponAttribute`</param>
        /// <returns>`Type` of `TTTReborn.Weapons.Weapon`</returns>
        public static Type GetWeaponTypeByName(string weaponName)
        {
            foreach (Type weaponType in GetWeapons())
            {
                if (GetWeaponName(weaponType) == weaponName)
                {
                    return weaponType;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an instance of a `TTTReborn.Weapons.Weapon` object by a `TTTReborn.Weapons.Weapon` `Type`.
        /// </summary>
        /// <param name="weaponType">A `TTTReborn.Weapons.Weapon` `Type`</param>
        /// <returns>Instance of a `TTTReborn.Weapons.Weapon` object</returns>
        public static TTTWeapon GetWeaponByType(Type weaponType)
        {
            return Library.Create<TTTWeapon>(weaponType);
        }

        /// <summary>
        /// Returns the `TTTReborn.Weapons.WeaponAttribute`'s `Name` of a given `TTTReborn.Weapons.Weapon`'s `Type`.
        /// </summary>
        /// <param name="weaponType">A `TTTReborn.Weapons.Weapon`'s `Type`</param>
        /// <returns>`TTTReborn.Weapons.WeaponAttribute`'s `Name`</returns>
        public static string GetWeaponName(Type weaponType)
        {
            return Library.GetAttribute(weaponType).Name;
        }
    }
}
