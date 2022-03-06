using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Map
{
    public partial class MapHandler
    {
        public MapSettings MapSettings { get; private set; }
        public List<AmmoRandom> RandomAmmos { get; private set; } = new();
        public List<WeaponRandom> RandomWeapons { get; private set; } = new();
        public List<LogicButton> LogicButtons { get; private set; } = new();

        public MapHandler()
        {
            foreach (Entity entity in Entity.All)
            {
                if (entity is MapSettings mapSettings)
                {
                    MapSettings = mapSettings;
                    MapSettings.FireSettingsSpawn();
                }

                Init(entity);
            }
        }

        public void Reset()
        {
            Sandbox.Internal.GlobalGameNamespace.Map.Reset(Game.DefaultCleanupFilter);
            Sandbox.Internal.Decals.RemoveFromWorld();

            RandomAmmos.Clear();
            RandomWeapons.Clear();
            LogicButtons.Clear();

            foreach (Entity entity in Entity.All)
            {
                Init(entity);
            }

            RandomWeapons.ForEach(x => x.Activate());
            RandomAmmos.ForEach(x => x.Activate());
            LogicButtons.ForEach(x => x.Cleanup());
        }

        private void Init(Entity entity)
        {
            if (entity is WeaponRandom weaponRandom)
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
}
