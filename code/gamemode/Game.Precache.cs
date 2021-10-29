using System;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;

namespace TTTReborn.Gamemode
{
    public partial class Game
    {
        public void PrecacheFiles()
        {
            Host.AssertServer();

            Precache.Add("particles/impact.generic.vpcf");

            foreach (Type type in Utils.GetTypesWithAttribute<Entity, PrecachedAttribute>())
            {
                PrecachedAttribute precachedAttribute = Utils.GetAttribute<PrecachedAttribute>(type);

                foreach (string precacheFile in precachedAttribute.PrecachedFiles)
                {
                    Precache.Add(precacheFile);
                }
            }
        }
    }
}
