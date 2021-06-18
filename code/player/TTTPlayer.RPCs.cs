using Sandbox;

using TTTReborn.Roles;
using TTTReborn.UI;

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

        [ClientRpc]
        public void ClientConfirmPlayer(TTTPlayer confirmPlayer, TTTPlayer deadPlayer, string roleName)
        {
            deadPlayer.Role = RoleFunctions.GetRoleByType(RoleFunctions.GetRoleTypeByName(roleName));

            Client confirmClient = confirmPlayer.GetClientOwner();
            Client deadClient = deadPlayer.GetClientOwner();

            InfoFeed.Current?.AddEntry(
                confirmClient,
                deadClient,
                "found the body of",
                $". Their role was {deadPlayer.Role.Name}!"
            );
        }

        [ClientRpc]
        public static void ClientOpenInspectMenu(TTTPlayer deadPlayer, bool isIdentified)
        {
            InspectMenu.Instance.InspectCorpse(deadPlayer, isIdentified);
        }

        [ClientRpc]
        public static void ClientCloseInspectMenu()
        {
            if (InspectMenu.Instance?.IsShowing ?? false)
            {
                InspectMenu.Instance.IsShowing = false;
            }
        }

        [ClientRpc]
        public void ClientDidDamage(Vector3 position, float amount, float inverseHealth)
        {
            Sound.FromScreen("dm.ui_attacker")
                .SetPitch(1 + inverseHealth * 1);
        }

        [ClientRpc]
        public void ClientTookDamage(Vector3 position)
        {

        }
    }
}
