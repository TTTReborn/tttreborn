using System;

using TTTReborn.Globals;
using TTTReborn.Roles;
using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    [Node("role_selection")]
    public class RoleSelectionNode : Node
    {
        public NodeRoleSelectionSetting RoleSelectionSetting;

        public RoleSelectionNode() : base(new RoleSelectionStackNode())
        {
            SetTitle("RoleSelection Node");

            RoleSelectionSetting = AddSetting<NodeRoleSelectionSetting>();

            HighlightError();
        }

        internal void OnSelectRole(Type roleType)
        {
            TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

            if (role == null)
            {
                return;
            }

            (StackNode as RoleSelectionStackNode).SelectedRole = role;

            Style.BackgroundColor = role.Color;
            Style.Dirty();
        }
    }
}
