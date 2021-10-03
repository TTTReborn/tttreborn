using Sandbox;

using TTTReborn.Events;
using TTTReborn.Items;
using TTTReborn.Map;
using TTTReborn.Player;
using TTTReborn.Roles;
using TTTReborn.Teams;
using TTTReborn.UI;

namespace TTTReborn.Globals
{
    public partial class RPCs
    {
        [ClientRpc]
        public static void ClientOnPlayerDied(TTTPlayer player)
        {
            if (!player.IsValid())
            {
                return;
            }

            Event.Run(TTTEvent.Player.Died, player);
        }

        [ClientRpc]
        public static void ClientOnPlayerConnected(Client client)
        {
            Event.Run(TTTEvent.Player.Connected, client);
        }

        [ClientRpc]
        public static void ClientOnPlayerDisconnect(ulong steamId, NetworkDisconnectionReason reason)
        {
            Event.Run(TTTEvent.Player.Disconnected, steamId, reason);
        }

        [ClientRpc]
        public static void ClientOnPlayerSpawned(TTTPlayer player)
        {
            if (!player.IsValid())
            {
                return;
            }

            player.IsMissingInAction = false;
            player.IsConfirmed = false;
            player.CorpseConfirmer = null;

            player.SetRole(new NoneRole());

            Event.Run(TTTEvent.Player.Spawned, player);
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

            player.SetRole(Utils.GetObjectByType<TTTRole>(Utils.GetTypeByName<TTTRole>(roleName)), TeamFunctions.GetTeam(teamName));
            player.SendRoleButtonsToClient();

            Client client = player.Client;

            if (client == null || !client.IsValid())
            {
                return;
            }

            Scoreboard.Instance.UpdateClient(client);
        }

        [ClientRpc]
        public static void ClientConfirmPlayer(TTTPlayer confirmPlayer, PlayerCorpse playerCorpse, TTTPlayer deadPlayer, string roleName, string teamName, ConfirmationData confirmationData, string killerWeapon, string[] perks)
        {
            if (!deadPlayer.IsValid())
            {
                return;
            }

            deadPlayer.SetRole(Utils.GetObjectByType<TTTRole>(Utils.GetTypeByName<TTTRole>(roleName)), TeamFunctions.GetTeam(teamName));

            deadPlayer.IsConfirmed = true;
            deadPlayer.CorpseConfirmer = confirmPlayer;

            if (playerCorpse.IsValid())
            {
                playerCorpse.Player = deadPlayer;
                playerCorpse.KillerWeapon = killerWeapon;
                playerCorpse.Perks = perks;

                playerCorpse.CopyConfirmationData(confirmationData);
                InspectMenu.Instance.SetPlayerData(playerCorpse);
            }

            Client deadClient = deadPlayer.Client;

            Scoreboard.Instance.UpdateClient(deadClient);

            if (!confirmPlayer.IsValid())
            {
                return;
            }

            Client confirmClient = confirmPlayer.Client;

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

            Scoreboard.Instance.UpdateClient(missingInActionPlayer.Client);
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
            PostRoundMenu.Instance.ClosePostRoundMenu();
        }

        [ClientRpc]
        public static void ClientOnPlayerCarriableItemPickup(Entity carriable)
        {
            Event.Run(TTTEvent.Player.Inventory.PickUp, carriable as ICarriableItem);
        }

        [ClientRpc]
        public static void ClientOnPlayerCarriableItemDrop(Entity carriable)
        {
            Event.Run(TTTEvent.Player.Inventory.Drop, carriable as ICarriableItem);
        }

        [ClientRpc]
        public static void ClientClearInventory()
        {
            Event.Run(TTTEvent.Player.Inventory.Clear);
        }

        [ClientRpc]
        public static void ClientDisplayMessage(string message, Color color)
        {
            InfoFeed.Current?.AddEntry(message, color);
        }
    }
}
