using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Map
{
    public partial class MapHandler
    {
        public MapSettings MapSettings { get; private set; }

        public int RandomWeaponCount, RandomAmmoCount;

        public List<AmmoRandom> RandomAmmos = new();
        public List<WeaponRandom> RandomWeapons = new();
        public List<LogicButton> LogicButtons = new();

        public MapHandler()
        {
            RandomWeaponCount = 0;

            foreach (Entity entity in Entity.All)
            {
                if (entity is MapSettings mapSettings)
                {
                    MapSettings = mapSettings;
                    MapSettings.FireSettingsSpawn();
                }
                else if (entity is WeaponRandom weaponRandom)
                {
                    RandomWeapons.Add(weaponRandom);
                }
                else if (entity is AmmoRandom ammoRandom)
                {
                    RandomAmmos.Add(ammoRandom);
                }
                else if (entity is LogicButton button)
                {
                    LogicButtons.Add(button);
                }
            }
        }

        public void Reset()
        {
            Sandbox.Internal.GlobalGameNamespace.Map.Reset(Game.DefaultCleanupFilter);
		    Sandbox.Internal.Decals.RemoveFromWorld();

            RandomWeapons.ForEach(x => x.Activate());
            RandomAmmos.ForEach(x => x.Activate());
            LogicButtons.ForEach(x => x.Cleanup());
        }
    }
}
