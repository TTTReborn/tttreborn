using Sandbox;
using System.Collections.Generic;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        private readonly List<ModelEntity> _clothing = new();

        public ModelEntity AttachClothing(string modelName)
        {
            ModelEntity entity = new ModelEntity();

            entity.SetModel(modelName);
            entity.SetParent(this, true);
            entity.EnableShadowInFirstPerson = true;
            entity.EnableHideInFirstPerson = true;

            _clothing.Add(entity);

            return entity;
        }

        public void RemoveClothing()
        {
            _clothing.ForEach(entity => entity.Delete());
            _clothing.Clear();
        }
    }

}
