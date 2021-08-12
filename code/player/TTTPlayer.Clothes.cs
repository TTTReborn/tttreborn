using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private readonly List<ModelEntity> _clothing = new();

        public ModelEntity AttachClothing(string modelName)
        {
            ModelEntity entity = new();
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
