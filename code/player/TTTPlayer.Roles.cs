using TTTReborn.Roles;
using TTTReborn.Teams;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        public TTTRole Role { get; private set; } = new NoneRole();

        public TTTTeam Team { get; private set; }

        public void SetRole(TTTRole role, TTTTeam team = null)
        {
            TTTTeam oldTeam = Team;

            Role?.OnDeselect(this);

            Role = role;
            Team = team ?? Role.DefaultTeam;

            if (oldTeam != Team)
            {
                oldTeam?.Members.Remove(this);
                Team?.Members.Add(this);
            }

            Role.OnSelect(this);
        }
    }
}
