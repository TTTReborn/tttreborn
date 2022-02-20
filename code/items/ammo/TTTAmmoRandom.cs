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
            List<Type> ammoTypes = Utils.GetTypesWithAttribute<TTTAmmo, SpawnableAttribute>();

            if (ammoTypes.Count <= 0)
            {
                return;
            }

            Type typeToSpawn = ammoTypes[Utils.RNG.Next(ammoTypes.Count)];

            TTTAmmo ent = Utils.GetObjectByType<TTTAmmo>(typeToSpawn);
            ent.Position = Position;
            ent.Rotation = Rotation;
            ent.Spawn();
        }
    }
}
