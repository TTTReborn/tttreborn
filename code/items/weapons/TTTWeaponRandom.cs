using System;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_random")]
    public class TTTWeaponRandom : Entity
    {
        public void Activate()
        {
            List<Type> wepTypes = Globals.Utils.GetTypes<TTTWeapon>(w => !w.IsDefined(typeof(NonSpawnableAttribute), true));

            if (wepTypes.Count > 0)
            {
                Type typeToSpawn = wepTypes[new Random().Next(wepTypes.Count)];
                TTTWeapon ent = Globals.Utils.GetObjectByType<TTTWeapon>(typeToSpawn);
                ent.Position = Position;
                ent.Rotation = Rotation;
                ent.Spawn();
            }
        }
    }
}
