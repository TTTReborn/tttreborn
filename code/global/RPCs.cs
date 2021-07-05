using Sandbox;

using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Globals
{
    using Items;

    using Roles;

    using Teams;

    public partial class RPCs
    {
        [ClientRpc]
        public static void ClientOnPlayerDied(TTTPlayer player)
        {
            if (!player.IsValid())
            {
                return;
            }

            Event.Run("tttreborn.player.died", player);
        }

        [ClientRpc]
        public static void ClientOnPlayerSpawned(TTTPlayer player)
        {
            if (!player.IsValid())
            {
                return;
            }

            Event.Run("tttreborn.player.spawned", player);

            player.IsMissingInAction = false;
            player.IsConfirmed = false;
            player.CorpseConfirmer = null;

            player.SetRole(new NoneRole());

            Hud.Current.GeneralHudPanel.Scoreboard.UpdatePlayer(player.GetClientOwner());
        }

        [ClientRpc]
        public static void ClientOnPlayerCarriableItemPickup(TTTPlayer player, Entity carriable)
        {
            if (!player.IsValid())
            {
                return;
            }

            Event.Run("tttreborn.player.carriableitem.pickup", carriable as ICarriableItem);
        }

        [ClientRpc]
        public static void ClientOnPlayerCarriableItemDrop(TTTPlayer player, Entity carriable)
        {
            if (!player.IsValid())
            {
                return;
            }

            Event.Run("tttreborn.player.carriableitem.drop", carriable as ICarriableItem);
        }

        /// <summary>
        /// Must be called on the server, updates TTTPlayer's `Role`.
        /// </summary>
        /// <param name="player">The player whose `Role` is to be updated</param>
        /// <param name="roleName">Same as the `TTTReborn.Roles.TTTRole`'s `TTTReborn.Roles.RoleAttribute`'s name</param>
        /// <param name="teamName">The name of the team</param>
        [ClientRpc]
        public static void ClientSetRole(TTTPlayer player, string roleName, string teamName = null)
        {
            if (!player.IsValid())
            {
                return;
            }

            player.SetRole(Utils.GetObjectByType<TTTRole>(Utils.GetTypeByName<TTTRole>(roleName)), TTTTeam.GetTeam(teamName));
        }

        [ClientRpc]
        public static void ClientConfirmPlayer(TTTPlayer confirmPlayer, TTTPlayer deadPlayer, string roleName, string teamName = null)
        {
            if (!confirmPlayer.IsValid() || !deadPlayer.IsValid())
            {
                return;
            }

            deadPlayer.SetRole(Utils.GetObjectByType<TTTRole>(Utils.GetTypeByName<TTTRole>(roleName)), TTTTeam.GetTeam(teamName));

            deadPlayer.IsConfirmed = true;
            deadPlayer.CorpseConfirmer = confirmPlayer;

            Client confirmClient = confirmPlayer.GetClientOwner();
            Client deadClient = deadPlayer.GetClientOwner();

            InfoFeed.Current?.AddEntry(
                confirmClient,
                deadClient,
                "found the body of",
                $"({deadPlayer.Role.Name})"
            );

            if (confirmPlayer == Local.Pawn as TTTPlayer && deadPlayer.CorpseCredits > 0)
            {
                InfoFeed.Current?.AddEntry(
                    confirmClient,
                    $"found $ {deadPlayer.CorpseCredits} credits!"
                );
            }
        }

        [ClientRpc]
        public static void ClientAddMissingInAction(TTTPlayer missingInActionPlayer)
        {
            if (!missingInActionPlayer.IsValid())
            {
                return;
            }

            missingInActionPlayer.IsMissingInAction = true;

            Hud.Current.GeneralHudPanel.Scoreboard.UpdatePlayer(missingInActionPlayer.GetClientOwner());
        }

        [ClientRpc]
        public static void ClientOpenInspectMenu(TTTPlayer deadPlayer, bool isIdentified)
        {
            if (!deadPlayer.IsValid())
            {
                return;
            }

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
        public static void ClientOpenAndSetPostRoundMenu(string winningTeam, Color winningColor)
        {
            PostRoundMenu.Instance.OpenAndSetPostRoundMenu(new PostRoundStats(
                winningRole: winningTeam,
                winningColor: winningColor
            ));
        }

        [ClientRpc]
        public static void ClientClosePostRoundMenu()
        {
            PostRoundMenu.Instance.IsShowing = false;
        }

        [ClientRpc]
        public static void ClientDidDamage(Vector3 position, float amount, float inverseHealth)
        {
            Sound.FromScreen("dm.ui_attacker")
                .SetPitch(1 + inverseHealth * 1);
        }

        [ClientRpc]
        public static void ClientTookDamage(Vector3 position)
        {

        }
    }
}
