using System;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("entity_weapon_random")]
    public class TTTWeaponRandom : Entity
    {
        public void Activate()
        {
            List<Type> wepTypes = Globals.Utils.GetTypesWithAttribute<TTTWeapon, SpawnableAttribute>();

            if (wepTypes.Count <= 0)
            {
                return;
            }

            Type typeToSpawn = wepTypes[new Random().Next(wepTypes.Count)];
            TTTWeapon ent = Globals.Utils.GetObjectByType<TTTWeapon>(typeToSpawn);
            ent.Position = Position;
            ent.Rotation = Rotation;
            ent.Spawn();
        }
    }
}
