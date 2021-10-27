using System;

using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Roles;

namespace TTTReborn.UI.VisualProgramming
{
    [NodeSetting("role_selection")]
    public class NodeRoleSelectionSetting : NodeSetting
    {
        public Dropdown Dropdown;

        public NodeRoleSelectionSetting() : base()
        {
            Content.SetPanelContent((panelContent) =>
            {
                Dropdown = panelContent.Add.Dropdown("roleselection");

                foreach (Type roleType in Utils.GetTypes<TTTRole>())
                {
                    Dropdown.AddOption($"{Utils.GetLibraryName(roleType).ToUpper()}_NAME", roleType, (panel) => OnSelectRole(roleType));
                }
            });
        }

        public void OnSelectRole(Type roleType)
        {
            if (Node is RoleSelectionNode roleSelectionNode)
            {
                roleSelectionNode.OnSelectRole(roleType);
            }

            Node?.RemoveHighlights();
        }
    }
}
