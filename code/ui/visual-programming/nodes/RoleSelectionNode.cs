using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI.VisualProgramming
{
    [Node("role_selection")]
    public class RoleSelectionNode : Node
    {
        public NodeRoleSelectionSetting RoleSelectionSetting;

        public RoleSelectionNode() : base()
        {
            SetTitle("RoleSelection Node");

            RoleSelectionSetting = AddSetting<NodeRoleSelectionSetting>();

            HighlightError();
        }

        public override void Evaluate(params object[] input)
        {
            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return;
            }

            if (RoleSelectionSetting.SelectedRoleType == null)
            {
                if (VisualProgrammingWindow.Instance?.IsTesting ?? false)
                {
                    HighlightError();

                    Log.Warning("No selected role in RoleSelectionNode.");
                }
                else
                {
                    Log.Error("No selected role in RoleSelectionNode.");
                }

                return;
            }

            if (!VisualProgrammingWindow.Instance?.IsTesting ?? true)
            {
                foreach (TTTPlayer player in playerList)
                {
                    Log.Error($"Selected '{player.Client.Name}' with role '{Utils.GetLibraryName(RoleSelectionSetting.SelectedRoleType)}'");
                }
            }

            base.Evaluate();
        }

        internal void OnSelectRole(Type roleType)
        {
            TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

            if (role == null)
            {
                return;
            }

            Style.BackgroundColor = role.Color;
            Style.Dirty();
        }
    }
}
