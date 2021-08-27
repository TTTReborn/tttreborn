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

        //Should probably be a ServerVar but I'm scared of giving control to players over this.
        public const int MAXIMUMSLOTS = 4; 

        public DNARegistry()
        {
            Instance = this;
        }

        public List<DNAInstance> Registry = new List<DNAInstance>();

        public void Track(DNAInstance instance)
        {
            if (!Registry.Contains(instance))
            {
                Registry.Add(instance);
            }
        }

        //Called by the DNAScanner to untrack a potential collection of DNAInstances.
        public void Untrack(IEnumerable<DNAInstance> instances)
        {
            Registry = Registry.Except(instances).ToList();
        }

        //Grabs DNA entries that match our passed `ent`. Double checks the expiration date just incase `OnSecond` didn't catch it.
        public List<DNAInstance> GetDNA(Entity ent) => Registry.Where(x => (x.Entity.Equals(ent)) && x.Expiration > 0).ToList();

        public void Clear()
        {
            Registry.Clear();
        }

        public void OnSecond()
        {
            //Funky-goofy way of removing elements out of a collection while iterating through it.
            //I genuinely don't think there is a way to get around doing this without exceptions.
            List<DNAInstance> instancesToRemove = new();

            foreach(var instance in Registry)
            {
                //Expiration is `RealTimeUntil` which is set to X amount of seconds from the date of creation.
                //This automatically is calculated somewhere within S&box for our easy use.
                if (instance.Expiration <= 0)
                {
                    //Optional callback function incase you want to get funny with it.
                    instance.Callback?.Invoke();
                    instancesToRemove.Add(instance);
                }
            }

            Registry = Registry.Except(instancesToRemove).ToList();
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
