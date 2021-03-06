using System;

using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Roles;

namespace TTTReborn.UI.VisualProgramming
{
    [NodeSetting("role_selection"), HideInEditor]
    public class NodeRoleSelectionSetting : NodeSetting
    {
        public TranslationDropdown Dropdown;

        public NodeRoleSelectionSetting() : base()
        {
            Content.SetPanelContent((panelContent) =>
            {
                Dropdown = panelContent.Add.TranslationDropdown();

                Dropdown.AddEventListener("onchange", (e) =>
                {
                    if (Dropdown?.Selected?.Value is Type roleType)
                    {
                        OnSelectRole(roleType);
                    }
                });

                foreach (Type roleType in Utils.GetTypes<Role>())
                {
                    Dropdown.Options.Add(new TranslationOption(new TranslationData(Utils.GetTranslationKey(Utils.GetLibraryName(roleType), "NAME")), roleType));
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
