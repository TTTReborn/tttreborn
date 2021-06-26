using System.Collections.Generic;

using Sandbox;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.Teams
{
    public class TTTTeam : Networked
    {
        public static readonly Dictionary<string, TTTTeam> Teams = new();

        public string Name { get; private set; }

        public Color Color { get; set; } = Color.Transparent;

        public readonly List<TTTPlayer> Members;

        public TTTTeam(string name)
        {
            Members = new();

            Name = name;
        }

        public static TTTTeam AddTeam(string name)
        {
            TTTTeam team = new TTTTeam(name);

            Teams.Add(name, team);

            return team;
        }

        public static TTTTeam GetTeam(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!Teams.TryGetValue(name, out TTTTeam team))
            {
                team = AddTeam(name);
            }

            return team;
        }
    }
}
