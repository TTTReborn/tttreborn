using System.Collections.Generic;

using Sandbox;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.VisualProgramming
{
    public partial class RoleSelectionStackNode : StackNode
    {
        public TTTRole SelectedRole { get; set; }

        public RoleSelectionStackNode() : base()
        {

        }

        public override object[] Build(params object[] input)
        {
            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return null;
            }

            if (SelectedRole == null)
            {
                throw new NodeStackException("No selected role in RoleSelectionNode.");
            }

            foreach (TTTPlayer player in playerList)
            {
                Log.Info($"Selected '{player.Client.Name}' with role '{SelectedRole.Name}'");
            }

            return base.Build(input[0]);
        }
    }
}
