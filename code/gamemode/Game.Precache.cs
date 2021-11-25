// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;

using Sandbox;

using TTTReborn.Events;
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

            Event.Run(TTTEvent.Game.Precache);
        }
    }
}
