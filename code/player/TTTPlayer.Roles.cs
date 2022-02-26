using System.Collections.Generic;

using Sandbox;

using TTTReborn.Roles;
using TTTReborn.Rounds;
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
                    _team = TeamFunctions.GetTeam(typeof(NoneTeam));
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

        /// <summary>
        /// Sends the role + team and all connected additional data like logic buttons of the current TTTPlayer to the given target or - if no target was provided - the player itself
        /// </summary>
        /// <param name="to">optional - The target</param>
        public void SendClientRole(To? to = null)
        {
            RPCs.ClientSetRole(to ?? To.Single(this), this, Role.Name);

            if (to == null || to.Value.ToString().Equals(Client.Name))
            {
                SendLogicButtonsToClient();
            }
        }

        public void SyncMIA(TTTPlayer player = null)
        {
            if (Gamemode.Game.Instance.Round is not InProgressRound)
            {
                return;
            }

            if (player == null)
            {
                List<Client> traitors = new();

                foreach (Client client in Client.All)
                {
                    if ((client.Pawn as TTTPlayer).Team.GetType() == typeof(TraitorTeam))
                    {
                        traitors.Add(client);
                    }
                }

                RPCs.ClientAddMissingInAction(To.Multiple(traitors), this);
            }
            else
            {
                RPCs.ClientAddMissingInAction(To.Single(player), this);
            }
        }
    }
}
