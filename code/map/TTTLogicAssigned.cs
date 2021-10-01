using Sandbox;

using TTTReborn.Player;
using TTTReborn.Rounds;

namespace TTTReborn.Map
{
    [Library("ttt_logic_assigned", Description = "Used to test the assigned team or role of the activator.")]
    public partial class TTTLogicAssigned : Entity
    {
        [Property("Check Type", "Entity will only return a result for one of the select values.")]
        public Check CheckType { get; set; } = Check.Role;

        [Property("Check Value", "Note that teams are often plural. For example, check the `Role` for `Traitor`, but check the `Team` for `Traitors`.")]
        public string CheckValue { get; set; } = "Traitor";

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
            if (activator is TTTPlayer player && Gamemode.Game.Instance.Round is InProgressRound)
            {
                if (CheckType == Check.Role)
                {
                    if (player.Role.Name.ToLower() == CheckValue.ToLower())
                    {
                        OnPass.Fire(this);
                        return;
                    }
                }
                else //CheckType == Check.Team
                {
                    if (player.Team.Name.ToLower() == CheckValue.ToLower())
                    {
                        OnPass.Fire(this);
                        return;
                    }
                }

                OnFail.Fire(this);
            }
            else
            {
                Log.Warning("ttt_logic_assigned: Activator is not player.");
                OnFail.Fire(this);
            }
        }
    }

    public enum Check
    {
        Role,
        Team
    }
}
