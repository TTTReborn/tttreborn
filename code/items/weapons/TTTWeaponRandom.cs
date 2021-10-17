using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Extensions;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_random")]
    public class TTTWeaponRandom : Entity
    {

        private static int AMMO_DISTANCE_UP = 24;

        /// <summary>
        /// Defines the amount of matching ammo entities that should be spawned near the weapons.
        /// </summary>
        [Property(Title = "Amount of Ammo")]
        public int AmmoToSpawn { get; set; } = 0;

        public void Activate()
        {
            List<Type> wepTypes = Globals.Utils.GetTypes<TTTWeapon>(w => !w.HasAttribute<NonSpawnableAttribute>(true));

            if (wepTypes.Count < 1)
            {
                Log.Error("No spawnable weapon entity found!");
                return;
            }

            Type weaponTypeToSpawn = Globals.Utils.RNG.FromList(wepTypes);
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
                TTTAmmo ammo = Globals.Utils.GetObjectByType<TTTAmmo>(weapon.AmmoEntity);

                // Siasur [15.10.2021]: Tried to bring in some variance when spawning ammo, couldn't really achieve the desired effect but will keep this for now.
                ammo.Position = weapon.Position + (Vector3.Up * AMMO_DISTANCE_UP);
                ammo.Velocity = Vector3.Random * AMMO_DISTANCE_UP;
                ammo.Rotation = Rotation.From(Angles.Random * 16);
                ammo.Spawn();
            }

        }
    }
}
