// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

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
        }

        public override bool Build(params object[] input)
        {
            (StackNode as RoleSelectionStackNode).SelectedRole = SelectedRole;

            return base.Build(input);
        }

        public override Dictionary<string, object> GetJsonData(List<Node> proceedNodes = null)
        {
            Dictionary<string, object> dict = base.GetJsonData(proceedNodes);
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
                        nodeRoleSelectionSetting.Dropdown?.SelectByData(roleType);
                        nodeRoleSelectionSetting.OnSelectRole(roleType);
                    }
                }
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
