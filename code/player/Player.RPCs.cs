using Sandbox;

using TTTReborn.Roles;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        [ClientRpc]
        public static void SetRole(string roleName)
        {
            Log.Warning($"Role: {roleName}");

            Instance.Role = RoleFunctions.GetRoleByType(RoleFunctions.GetRoleTypeByName(roleName));
        }
    }
}
