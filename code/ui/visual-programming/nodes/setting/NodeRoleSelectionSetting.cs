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
                    string name = Utils.GetLibraryName(roleType);

                    dropdown.AddOption(name, roleType, (panel) =>
                    {
                        SelectedRoleType = roleType;
                    });
                }
            });
        }
    }
}
