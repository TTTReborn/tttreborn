using System.Linq;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Map
{
    public partial class MapHandler
    {
        public MapSettings MapSettings { get; private set; }
        public List<Entity> Ammos { get; private set; } = new();
        public List<Entity> Weapons { get; private set; } = new();
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

            Ammos.Clear();
            Weapons.Clear();
            LogicButtons.Clear();

            foreach (Entity entity in Entity.All)
            {
                Init(entity);
            }

            foreach (Entity weapon in Weapons)
            {
                if (weapon is WeaponRandom weaponRandom)
                {
                    weaponRandom.Activate();
                }
            }

            foreach (Entity ammo in Ammos)
            {
                if (ammo is AmmoRandom ammoRandom)
                {
                    ammoRandom.Activate();
                }
            }

            LogicButtons.ForEach(x => x.Reset());
        }

        private void Init(Entity entity)
        {
            if (entity is WeaponRandom || entity is Weapon)
            {
                Weapons.Add(entity);
            }
            else if (entity is AmmoRandom || entity is Ammo)
            {
                Ammos.Add(entity);
            }
            else if (entity is LogicButton button)
            {
                LogicButtons.Add(button);
            }
        }
    }
}
