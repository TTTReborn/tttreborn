using System;
using System.Collections.Generic;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.VisualProgramming
{
    [StackNode("role_selection")]
    public partial class RoleSelectionStackNode : StackNode
    {
        public TTTRole SelectedRole { get; set; }

        public RoleSelectionStackNode() : base()
        {

        }

        public override object[] Test(object[] input)
        {
            if (SelectedRole == null)
            {
                throw new NodeStackException("No selected role in RoleSelectionNode.");
            }

            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return null;
            }

            foreach (TTTPlayer player in playerList)
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
            foreach (TTTPlayer player in input[0] as List<TTTPlayer>)
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
