using System.Collections.Generic;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.UI.VisualProgramming
{
    [Node("conditional_placeholder")]
    public class ConditionalPlaceholderNode : Node
    {
        public NodeRoleSelectionSetting RoleSelectionSetting;

        public ConditionalPlaceholderNode() : base()
        {
            SetTitle("Conditional Placeholder Node");

            RoleSelectionSetting = AddSetting<NodeRoleSelectionSetting>();
        }

        public override void Evaluate(params object[] input)
        {
            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return;
            }

            foreach (TTTPlayer player in playerList)
            {
                Sandbox.Log.Error($"Selected '{player.Client.Name}' with role '{Utils.GetLibraryName(RoleSelectionSetting.SelectedRoleType)}'");
            }
        }
    }
}
