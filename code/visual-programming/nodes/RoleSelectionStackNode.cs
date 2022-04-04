using System;
using System.Collections.Generic;

using TTTReborn.Roles;

namespace TTTReborn.VisualProgramming
{
    [StackNode("role_selection")]
    public partial class RoleSelectionStackNode : StackNode
    {
        public Role SelectedRole { get; set; }

        public override object[] Test(object[] input)
        {
            if (SelectedRole == null)
            {
                throw new NodeStackException("No selected role in RoleSelectionNode.");
            }

            if (input == null || input[0] is not List<Player> playerList)
            {
                return null;
            }

            foreach (Player player in playerList)
            {
                Log.Debug($"Selected '{player.Client.Name}' with role '{SelectedRole.Name}'");
            }

            return new object[]
            {
                input[0]
            };
        }

        public override object[] Evaluate(object[] input)
        {
            foreach (Player player in input[0] as List<Player>)
            {
                player.SetRole(SelectedRole);
            }

            return new object[]
            {
                input[0]
            };
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
                Type roleType = Utils.GetTypeByLibraryName<Role>(selectedRoleName.ToString());

                if (roleType != null)
                {
                    SelectedRole = Utils.GetObjectByType<Role>(roleType);
                }
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
