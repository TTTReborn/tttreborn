using System;

using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Roles;

namespace TTTReborn.UI.VisualProgramming
{
    [NodeSetting("role_selection")]
    public class NodeRoleSelectionSetting : NodeSetting
    {
        public Type SelectedRoleType { get; set; }

        public NodeRoleSelectionSetting() : base()
        {
            Content.SetPanelContent((panelContent) =>
            {
                Dropdown dropdown = panelContent.Add.Dropdown("roleselection");

                foreach (Type roleType in Utils.GetTypes<TTTRole>())
                {
                    dropdown.AddOption(Utils.GetLibraryName(roleType), roleType, (panel) => OnSelectRole(roleType));
                }
            });
        }

        private void OnSelectRole(Type roleType)
        {
            SelectedRoleType = roleType;

            if (Node is RoleSelectionNode roleSelectionNode)
            {
                roleSelectionNode.OnSelectRole(roleType);
            }

            Node?.RemoveHighlights();
        }
    }
}
