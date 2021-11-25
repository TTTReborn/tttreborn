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
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.Teams
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TeamAttribute : LibraryAttribute
    {
        public TeamAttribute(string name) : base("team_" + name)
        {

        }
    }

    public abstract class TTTTeam
    {
        public readonly string Name;

        public abstract Color Color { get; }

        public readonly List<TTTPlayer> Members = new();

        public static Dictionary<string, TTTTeam> Teams = new();

        public TTTTeam()
        {
            Name = Utils.GetLibraryName(GetType());

            Teams[Name] = this;
        }

        public IEnumerable<Client> GetClients()
        {
            return Members.Select(x => x.Client);
        }
    }

    public static class TeamFunctions
    {
        public static TTTTeam TryGetTeam(string teamname)
        {
            if (teamname == null || !TTTTeam.Teams.TryGetValue(teamname, out TTTTeam team))
            {
                return null;
            }

            return team;
        }

        public static TTTTeam GetTeam(string teamname)
        {
            if (teamname == null)
            {
                return null;
            }

            if (!TTTTeam.Teams.TryGetValue(teamname, out TTTTeam team))
            {
                team = Utils.GetObjectByType<TTTTeam>(Utils.GetTypeByLibraryName<TTTTeam>(teamname));
            }

            return team;
        }

        public static TTTTeam GetTeam(Type teamType)
        {
            foreach (TTTTeam team in TTTTeam.Teams.Values)
            {
                if (team.GetType() == teamType)
                {
                    return team;
                }
            }

            return Utils.GetObjectByType<TTTTeam>(teamType);
        }
    }
}
