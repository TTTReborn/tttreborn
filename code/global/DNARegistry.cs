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

        public const int MAXIMUMSLOTS = 4; 

        public DNARegistry()
        {
            Instance = this;
        }

        public List<DNAInstance> Registry = new List<DNAInstance>();

        public void Track(DNAInstance instance)
        {
            Registry.Add(instance);
        }

        public void Untrack(IEnumerable<DNAInstance> instances)
        {
            Registry = Registry.Except(instances).ToList();
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
        public DNAType Type;
        public TTTPlayer Player;
        public RealTimeUntil Expiration;
        public Action Callback;

        public DNAInstance(Entity ent, DNAType type, TTTPlayer player, RealTimeUntil? expiration = null, Action callback = null)
        {
            Entity = ent;
            Type = type;
            Player = player;
            Expiration = expiration ?? 60;
            Callback = callback;
        }
    }

    //Eventually, much later down the line, this should be phased out and replaced by grabbing the icon off the weapon/equipment itself.
    //Then create a dedicated "player" icon.
    //Works with the stands ins for the moment.
    public enum DNAType
    {
        Corpse,
        Weapon,
        Ammo
    }
}
