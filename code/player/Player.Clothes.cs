using Sandbox;
using System.Collections.Generic;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        List<ModelEntity> Clothing = new();

        public ModelEntity AttachClothing(string modelName)
        {
            var entity = new ModelEntity();

            entity.SetModel(modelName);
            entity.SetParent(this, true);
            entity.EnableShadowInFirstPerson = true;
            entity.EnableHideInFirstPerson = true;

            Clothing.Add(entity);

            return entity;
        }

        public void RemoveClothing()
        {
            Clothing.ForEach(entity => entity.Delete());
            Clothing.Clear();
        }
    }

}
