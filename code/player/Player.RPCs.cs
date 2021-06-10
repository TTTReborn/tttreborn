using Sandbox;

using TTTReborn.Roles;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        /// <summary>
        /// Called by the server, updates client's `Role`.
        /// </summary>
        /// <param name="roleName">Same as the `TTTReborn.Roles.BaseRole`'s `TTTReborn.Roles.RoleAttribute`'s name</param>
        [ClientRpc]
        public void SetRole(string roleName)
        {
            Role = RoleFunctions.GetRoleByType(RoleFunctions.GetRoleTypeByName(roleName));
        }
    }
}
