using Sandbox;

using TTTReborn.Player;
using TTTReborn.Rounds;
using TTTReborn.Teams;

namespace TTTReborn.Map
{
    [Library("ttt_force_win", Description = "Forces round to end and win be awarded to team depending on input.")]
    public partial class TTTForceWin : Entity
    {
        [Property("Team", "The name of the team that will be forced to win. This entity also contains built in inputs for certain teams. Use this for setting win conditions for custom teams.")]
        public string Team
        {
            get => _team;
            set
            {
                _team = value?.ToLower();
            }
        }
        private string _team = Utils.GetLibraryName(typeof(InnocentTeam));

        [Property("Use Activators Team", "OVERRIDES `Team` PROPERTY. When ForceWin() is fired, this will award a win to the team of the activating player.")]
        public bool UseActivatorsTeam { get; set; } = false;

        [Input]
        public void InnocentsWin() => ForceEndRound(TeamFunctions.GetTeam(typeof(InnocentTeam)));

        [Input]
        public void TraitorsWin() => ForceEndRound(TeamFunctions.GetTeam(typeof(TraitorTeam)));

        [Input]
        public void ForceWin(Entity activator)
        {
            TTTTeam winningTeam;

            if (UseActivatorsTeam && activator is TTTPlayer player)
            {
                winningTeam = player.Team;
            }
            else
            {
                winningTeam = TeamFunctions.TryGetTeam(Team);
            }

            if (winningTeam != null)
            {
                ForceEndRound(winningTeam);

                return;
            }

            Log.Warning($"ttt_force_win: Failed to grant win to team: {Team}, invalid or nonexistant team name.");
        }

        private static void ForceEndRound(TTTTeam team)
        {
            if (Gamemode.Game.Instance.Round is InProgressRound)
            {
                //Logic taken from InProgressRound.LoadPostRound. Should reference the function instead?
                Gamemode.Game.Instance.ForceRoundChange(new PostRound());
                RPCs.ClientOpenAndSetPostRoundMenu(
                    team.Name,
                    team.Color
                );
            }
        }
    }
}
