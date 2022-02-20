using System;
using System.Collections.Generic;

using TTTReborn.Roles;
using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    [Spawnable]
    [Node("role_selection")]
    public class RoleSelectionNode : Node
    {
        public TTTRole SelectedRole { get; set; }

        public RoleSelectionNode() : base(new RoleSelectionStackNode())
        {
            SetTitle("RoleSelection Node");

            AddSetting<NodeRoleSelectionSetting>();
        }

        internal void OnSelectRole(Type roleType)
        {
            TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

            if (role == null)
            {
                return;
            }

            SelectedRole = role;

            Style.BackgroundColor = role.Color;
        }

        public override void Prepare()
        {
            (StackNode as RoleSelectionStackNode).SelectedRole = SelectedRole;

            base.Prepare();
        }

        public override Dictionary<string, object> GetJsonData()
        {
            Dictionary<string, object> dict = base.GetJsonData();
            dict.Add("SelectedRole", SelectedRole?.Name);

            return dict;
        }

        public override void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("SelectedRole", out object selectedRoleName);

            if (selectedRoleName != null)
            {
                Type roleType = Utils.GetTypeByLibraryName<TTTRole>(selectedRoleName.ToString());

                if (roleType != null)
                {
                    if (NodeSettings[0] is NodeRoleSelectionSetting nodeRoleSelectionSetting)
                    {
                        nodeRoleSelectionSetting.Dropdown?.Select(roleType);
                        nodeRoleSelectionSetting.OnSelectRole(roleType);
                    }
                }
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
