using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Globals
{
    public partial class DNARegistry
    {
        public static DNARegistry Instance;

        public DNARegistry()
        {
            Instance = this;
        }

        public List<DNAInstance> Registry = new List<DNAInstance>();

        public void Track(DNAInstance instance)
        {
            Registry.Add(instance);
        }

        public List<DNAInstance> GetDNA(Entity ent) => Registry.Where(x => (x.Entity.Equals(ent)) && x.Expiration > 0).ToList();

        public void Clear()
        {
            Registry.Clear();
        }

        public void OnSecond()
        {
            List<DNAInstance> instancesToRemove = new();

            foreach(var instance in Registry)
            {

                if (instance.Expiration <= 0)
                {
                    instance.Callback?.Invoke();
                    instancesToRemove.Add(instance);
                }
            }

            Registry.RemoveAll(x => instancesToRemove.Contains(x));
        }
    }

    public struct DNAInstance
    {
        public Entity Entity;
        public TTTPlayer Player;
        public RealTimeUntil Expiration;
        public Action Callback;

        public DNAInstance(Entity ent, TTTPlayer player, RealTimeUntil? expiration = null, Action callback = null)
        {
            Entity = ent;
            Player = player;
            Expiration = expiration ?? 60;
            Callback = callback;
        }
    }
}
