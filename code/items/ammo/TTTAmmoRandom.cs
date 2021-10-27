using System;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("entity_ammo_random")]
    public class TTTAmmoRandom : Entity
    {
        public void Activate()
        {
            List<Type> ammoTypes = Globals.Utils.GetTypesWithAttribute<TTTAmmo, SpawnableAttribute>();

            if (ammoTypes.Count <= 0)
            {
                return;
            }

            Type typeToSpawn = ammoTypes[new Random().Next(ammoTypes.Count)];
            TTTAmmo ent = Globals.Utils.GetObjectByType<TTTAmmo>(typeToSpawn);
            ent.Position = Position;
            ent.Rotation = Rotation;
            ent.Spawn();
        }
    }
}
