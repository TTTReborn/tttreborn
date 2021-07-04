using System.Collections.Generic;

using Sandbox;

using TTTReborn.Roles;
using TTTReborn.Teams;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public TTTRole Role
        {
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

        public TTTTeam Team
        {
            get
            {
                if (_team == null)
                {
                    _team = TTTTeam.GetTeam("Nones");
                }

                return _team;
            }
            private set
            {
                _team = value;
            }
        }

        private TTTTeam _team;

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

        public void SyncMIA(TTTPlayer player = null)
        {
            if (Gamemode.Game.Instance.Round is not Rounds.InProgressRound)
            {
                return;
            }

            if (player == null)
            {
                List<Client> traitors = new();

                foreach (Client client in Client.All)
                {
                    if ((client.Pawn as TTTPlayer).Team.Name == "Traitors")
                    {
                        traitors.Add(client);
                    }
                }

                ClientAddMissingInAction(To.Multiple(traitors), this);
            }
            else
            {
                ClientAddMissingInAction(To.Single(player), this);
            }
        }
    }
}
