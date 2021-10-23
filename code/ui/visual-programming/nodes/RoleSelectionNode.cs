using System;
using System.Collections.Generic;

using TTTReborn.Globals;
using TTTReborn.Roles;
using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    [Node("role_selection")]
    public class RoleSelectionNode : Node
    {
        public TTTRole SelectedRole { get; set; }

        public RoleSelectionNode() : base(new RoleSelectionStackNode())
        {
            SetTitle("RoleSelection Node");

            AddSetting<NodeRoleSelectionSetting>();

            HighlightError();
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
            Style.Dirty();
        }

        public override void Build(params object[] input)
        {
            (StackNode as RoleSelectionStackNode).SelectedRole = SelectedRole;

            base.Build(input);
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
                    SelectedRole = Utils.GetObjectByType<TTTRole>(roleType);
                }
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
