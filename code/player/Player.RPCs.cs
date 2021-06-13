using Sandbox;
using Sandbox.UI;

using TTTReborn.Roles;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        [ClientRpc]
        public void ClientOnPlayerDied(TTTPlayer player)
        {
            Event.Run("tttreborn.player.died", player);
        }

        [ClientRpc]
        public void ClientOnPlayerSpawned(TTTPlayer player)
        {
            Event.Run("tttreborn.player.spawned", player);

            player.IsAlive = true;
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
            deadPlayer.IsAlive = false;

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
        public void DidDamage(Vector3 position, float amount, float inverseHealth)
        {
            Sound.FromScreen("dm.ui_attacker")
                .SetPitch(1 + inverseHealth * 1);
        }

        [ClientRpc]
        public void TookDamage(Vector3 position)
        {

        }
    }
}
