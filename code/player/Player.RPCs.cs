using Sandbox;

using TTTReborn.Roles;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        [ClientRpc]
        public void ClientOnPlayerDied()
        {
            Event.Run("tttreborn.player.died");
        }

        [ClientRpc]
        public void ClientOnPlayerSpawned()
        {
            Event.Run("tttreborn.player.spawned");
        }

        /// <summary>
        /// Must be called on the server, updates TTTPlayer's `Role`.
        /// </summary>
        /// <param name="roleName">Same as the `TTTReborn.Roles.BaseRole`'s `TTTReborn.Roles.RoleAttribute`'s name</param>
        [ClientRpc]
        public void ClientSetRole(string roleName)
        {
            Role = RoleFunctions.GetRoleByType(RoleFunctions.GetRoleTypeByName(roleName));
        }
    }
}
