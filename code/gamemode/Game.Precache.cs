using System;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Items;

namespace TTTReborn.Gamemode
{
    public partial class Game
    {
        public static void PrecacheFiles()
        {
            Host.AssertServer();

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

            Event.Run(TTTEvent.Game.PRECACHE);
        }
    }
}
