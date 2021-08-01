using System;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_random")]
    public class TTTWeaponRandom : Entity
    {
        public void Activate()
        {
            var wepTypes = Globals.Utils.GetTypes<TTTWeapon>();

            if (wepTypes.Count > 0)
            {
                var typeToSpawn = wepTypes[new Random().Next(wepTypes.Count)];
                TTTWeapon ent = Globals.Utils.GetObjectByType<TTTWeapon>(typeToSpawn);
                ent.Position = Position;
                ent.Rotation = Rotation;
                ent.Spawn();
            }
        }
    }
}
