using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Roles;
using TTTReborn.Teams;
using TTTReborn.UI;

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
                    _team = NoneTeam.Instance;
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
            Team = team ?? TeamFunctions.GetTeamByType(Role.DefaultTeamType);

            if (oldTeam != Team)
            {
                oldTeam?.Members.Remove(this);
                Team?.Members.Add(this);
            }

            Role.OnSelect(this);

            if (QuickShop.Instance != null)
            {
                QuickShop.Instance.CheckAccess();

                if (QuickShop.Instance.IsShowing)
                {
                    QuickShop.Instance.Update();
                }
            }
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

                RPCs.ClientAddMissingInAction(To.Multiple(traitors), this);
            }
            else
            {
                RPCs.ClientAddMissingInAction(To.Single(player), this);
            }
        }
    }
}
