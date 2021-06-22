using TTTReborn.Roles;
using TTTReborn.Teams;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        public TTTRole Role {
            get
            {
                if (_role == null)
                {
                    _role = new NoneRole();
                }

                return _role;
            }
            private set
            {
                _role = value;
            }
        }

        private TTTRole _role;

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
