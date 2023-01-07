using System;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Gamemode
{
    public partial class TTTGame
    {
        public static void PrecacheFiles()
        {
            Game.AssertServer();

            Precache.Add("particles/impact.generic.vpcf");
            Precache.Add("particles/impact.flesh.vpcf");
            Precache.Add("particles/impact.metal.vpcf");

            foreach (Type type in Utils.GetTypesWithAttribute<Entity, PrecachedAttribute>())
            {
                PrecachedAttribute precachedAttribute = Utils.GetAttribute<PrecachedAttribute>(type);

                foreach (string precacheFile in precachedAttribute.PrecachedFiles)
                {
                    Precache.Add(precacheFile);
                }
            }

            GameEvent.Register(new Events.Game.PrecacheEvent());
        }
    }
}
