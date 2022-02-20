using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Map
{
    public partial class MapHandler
    {
        public TTTMapSettings MapSettings { get; private set; }

        public List<ModelEntityData> ModelEntityDataList;
        public int RandomWeaponCount;

        public MapHandler()
        {
            ModelEntityDataList = new();
            RandomWeaponCount = 0;

            foreach (Entity entity in Entity.All)
            {
                if (entity is TTTMapSettings mapSettings)
                {
                    MapSettings = mapSettings;
                    MapSettings.FireSettingsSpawn();
                }
                else if (entity is Sandbox.Prop || entity is BaseCarriable)
                {
                    ModelEntityDataList.Add(ModelEntityData.Create(entity as ModelEntity));
                }
                else if (entity is TTTWeaponRandom)
                {
                    RandomWeaponCount++;
                }
            }
        }

        public void Reset()
        {
            List<TTTAmmoRandom> randomAmmo = new();
            List<TTTWeaponRandom> randomWeapons = new();

            foreach (Entity entity in Entity.All)
            {
                if (entity is Sandbox.Prop || entity is BaseCarriable || entity.Tags.Has(IItem.ITEM_TAG))
                {
                    entity.Delete();
                }
                else if (entity is TTTAmmoRandom ammoRandom)
                {
                    randomAmmo.Add(ammoRandom); // Throws `Collection was Modified` if we activate here. Worth looking further into cleanup wise.
                }
                else if (entity is TTTWeaponRandom weaponRandom)
                {
                    randomWeapons.Add(weaponRandom); // See above comment.
                }
                else if (entity is TTTLogicButton button)
                {
                    button.Cleanup();
                }
                else if (entity is PathPlatformEntity path)
                {
                    path.WarpToPoint(0);
                }

                entity.RemoveAllDecals();
            }

            foreach (ModelEntityData modelEntityData in ModelEntityDataList)
            {
                ModelEntity prop = Utils.GetObjectByType<ModelEntity>(modelEntityData.Type);
                prop.Position = modelEntityData.Position;
                prop.Rotation = modelEntityData.Rotation;
                prop.Scale = modelEntityData.Scale;
                prop.RenderColor = modelEntityData.Color;
                prop.Model = modelEntityData.Model;
                prop.Spawn();
            }

            randomAmmo.ForEach(x => x.Activate());
            randomWeapons.ForEach(x => x.Activate());
        }
    }

    public class ModelEntityData
    {
        public Type Type;
        public Vector3 Position;
        public Rotation Rotation;
        public float Scale;
        public Color Color;
        public Model Model;

        public static ModelEntityData Create(ModelEntity modelEntity)
        {
            return new()
            {
                Type = modelEntity.GetType(),
                Position = modelEntity.Position,
                Rotation = modelEntity.Rotation,
                Scale = modelEntity.Scale,
                Color = modelEntity.RenderColor,
                Model = modelEntity.Model,
            };
        }
    }
}
