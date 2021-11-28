using System;
using System.Collections.Generic;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.VisualProgramming
{
    public partial class RoleSelectionStackNode : StackNode
    {
        public TTTRole SelectedRole { get; set; }

        public RoleSelectionStackNode() : base()
        {

        }

        public override object[] Test(params object[] input)
        {
            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return null;
            }

            if (SelectedRole == null)
            {
                throw new NodeStackException("No selected role in RoleSelectionNode.");
            }

            foreach (TTTPlayer player in playerList)
            {
                Log.Info($"Selected '{player.Client.Name}' with role '{SelectedRole.Name}'");
            }

            return new object[]
            {
                input[0]
            };
        }

        public override Dictionary<string, object> GetJsonData(List<StackNode> proceedNodes = null)
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
                    SelectedRole = Utils.GetObjectByType<TTTRole>(roleType);
                }
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
