using Sandbox;

using TTTReborn.Rounds;
using TTTReborn.Teams;

namespace TTTReborn.Map
{
    [Library("ttt_logic_doorentity", Description = "Used to assign team or role to a door.")]
    public partial class RoleDoorEntity : DoorEntity
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

        public override bool IsUsable(Entity user)
        {
            return user is Player player && (player.Role.Name.Equals(CheckValue) || player.Team.Name.Equals(CheckValue));
        }

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
            if (Gamemode.Game.Instance.Round is InProgressRound && IsUsable(activator))
            {
                Toggle(activator);

                _ = OnPass.Fire(this);

                return;
            }

            _ = OnFail.Fire(this);
        }
    }
}
