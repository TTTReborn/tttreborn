using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

namespace TTTReborn.Teams
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TeamAttribute : LibraryAttribute
    {
        public TeamAttribute(string name) : base("ttt_team_" + name)
        {

        }
    }

    public abstract class Team
    {
        public readonly string Name;

        public abstract Color Color { get; }

        public List<Player> Members { get; set; } = new();

        public static Dictionary<string, Team> Teams { get; set; } = new();

        public Team()
        {
            Name = Utils.GetLibraryName(GetType());

            if (!Teams.ContainsKey(Name))
            {
                Teams[Name] = this;
            }
            else
            {
                Log.Warning($"You've created an instance of the Team '{Name}' that already exists. This will cause issues using the new instance. Instead, use `TeamFunctions.GetTeam()`.");
            }
        }

        public IEnumerable<Client> GetClients()
        {
            return Members.Select(x => x.Client);
        }

        public virtual bool CheckWin(Player player) => true;

        public virtual bool CheckPreventWin(Player player) => false;

        public string GetTranslationKey(string key = null) => Utils.GetTranslationKey(Name, key);

        public virtual bool Equals(Team team)
        {
            return Name.Equals(team.Name);
        }
    }

    public static class TeamFunctions
    {
        public static Team TryGetTeam(string teamname)
        {
            if (teamname == null || !Team.Teams.TryGetValue(teamname.ToLower(), out Team team))
            {
                return null;
            }

            return team;
        }

        public static Team GetTeam(string teamname)
        {
            if (string.IsNullOrEmpty(teamname))
            {
                return null;
            }

            Team team = TryGetTeam(teamname);

            if (team != null)
            {
                return team;
            }

            Type type = Utils.GetTypeByLibraryName<Team>(teamname);

            if (type == null)
            {
                return null;
            }

            return Utils.GetObjectByType<Team>(type);
        }

        public static Team GetTeam(Type teamType) => GetTeam(Utils.GetLibraryName(teamType));

        public static bool IsTeamMember(this Player self, Player other)
        {
            return self.Team == other.Team && self.Team.CheckWin(self);
        }
    }
}
