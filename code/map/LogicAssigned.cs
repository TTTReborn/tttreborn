using Sandbox;

using TTTReborn.Rounds;
using TTTReborn.Teams;

namespace TTTReborn.Map
{
    [Library("ttt_logic_assigned", Description = "Used to test the assigned team or role of the activator.")]
    public partial class LogicAssigned : Entity
    {
        [Property("Check Value", "Note that teams are often plural. For example, check the `Role` for `ttt_role_traitor`, but check the `Team` for `ttt_team_traitors`.")]
        public string CheckValue
        {
            get => _checkValue;
            set
            {
                _checkValue = value?.ToLower();
            }
        }
        private string _checkValue = Utils.GetLibraryName(typeof(TraitorTeam));

        /// <summary>
        /// Fires if activator's check type matches the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
        /// </summary>
        protected Output OnPass { get; set; }

        /// <summary>
        /// Fires if activator's check type does not match the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
        /// </summary>
        protected Output OnFail { get; set; }

        [Input]
        public void Activate(Entity activator)
        {
            if (activator is Player player && Gamemode.TTTGame.Instance.Round is InProgressRound)
            {
                if (player.Role.Name.Equals(CheckValue) || player.Team.Name.Equals(CheckValue))
                {
                    _ = OnPass.Fire(this);

                    return;
                }
            }

            _ = OnFail.Fire(this);
        }
    }
}
