using System;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_random")]
    public class TTTAmmoRandom : Entity
    {
        public void Activate()
        {
            var ammoTypes = Globals.Utils.GetTypes<TTTAmmo>();

            if (ammoTypes.Count > 0)
            {
                var typeToSpawn = ammoTypes[new Random().Next(ammoTypes.Count)];
                TTTAmmo ent = Globals.Utils.GetObjectByType<TTTAmmo>(typeToSpawn);
                ent.Position = Position;
                ent.Rotation = Rotation;
                ent.Spawn();
            }
        }
    }
}
