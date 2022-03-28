using Sandbox;

using TTTReborn.Roles;
using TTTReborn.Teams;
using TTTReborn.UI;

namespace TTTReborn.Globals
{
    public partial class RPCs
    {
        [ClientRpc]
        public static void ClientOnPlayerDied(Player player)
        {
            if (!player.IsValid())
            {
                return;
            }

            GameEvent.Register(new Events.Player.DiedEvent(player));
        }

        [ClientRpc]
        public static void ClientOnPlayerConnected(Client client)
        {
            GameEvent.Register(new Events.Player.ConnectedEvent(client));
        }

        [ClientRpc]
        public static void ClientOnPlayerDisconnect(long playerId, NetworkDisconnectionReason reason)
        {
            GameEvent.Register(new Events.Player.DisconnectedEvent(playerId, reason));
        }

        [ClientRpc]
        public static void ClientOnPlayerSpawned(Player player)
        {
            if (!player.IsValid())
            {
                return;
            }

            player.IsMissingInAction = false;
            player.IsConfirmed = false;
            player.CorpseConfirmer = null;

            player.SetRole(new NoneRole());

            GameEvent.Register(new Events.Player.SpawnEvent(player));
        }

        /// <summary>
        /// Must be called on the server, updates Player's `Role`.
        /// </summary>
        /// <param name="player">The player whose `Role` is to be updated</param>
        /// <param name="roleName">Same as the `TTTReborn.Roles.Role`'s `TTTReborn.Roles.RoleAttribute`'s name</param>
        /// <param name="teamName">The name of the team</param>
        [ClientRpc]
        public static void ClientSetRole(Player player, string roleName, string teamName = null)
        {
            if (!player.IsValid())
            {
                return;
            }

            player.SetRole(Utils.GetObjectByType<Role>(Utils.GetTypeByLibraryName<Role>(roleName)), TeamFunctions.GetTeam(teamName));

            Client client = player.Client;

            if (client == null || !client.IsValid())
            {
                return;
            }

            Scoreboard.Instance?.UpdateClient(client);
        }

        [ClientRpc]
        public static void ClientConfirmPlayer(Player confirmPlayer, PlayerCorpse playerCorpse, Player deadPlayer, string roleName, string teamName, ConfirmationData confirmationData, string killerWeapon, string[] perks, bool covert = false)
        {
            if (!deadPlayer.IsValid())
            {
                return;
            }

            deadPlayer.SetRole(Utils.GetObjectByType<Role>(Utils.GetTypeByLibraryName<Role>(roleName)), TeamFunctions.GetTeam(teamName));

            if (!covert)
            {
                deadPlayer.IsConfirmed = true;
                deadPlayer.CorpseConfirmer = confirmPlayer;
            }

            if (playerCorpse.IsValid())
            {
                playerCorpse.DeadPlayer = deadPlayer;
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

            if (confirmPlayer == Local.Pawn as Player && deadPlayer.CorpseCredits > 0)
            {
                InfoFeed.Current?.AddEntry(
                    confirmClient,
                    $"found $ {deadPlayer.CorpseCredits} credits!"
                );
            }
        }

        [ClientRpc]
        public static void ClientAddMissingInAction(Player missingInActionPlayer)
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
                winningTeam: winningTeam,
                winningColor: winningColor
            ));
        }

        [ClientRpc]
        public static void ClientClosePostRoundMenu()
        {
            PostRoundMenu.Instance.ClosePostRoundMenu();
        }

        [ClientRpc]
        public static void ClientOpenMapSelectionMenu()
        {
            MapSelectionMenu.Instance.Enabled(true);
        }

        [ClientRpc]
        public static void ClientOnPlayerCarriableItemPickup(Entity carriable)
        {
            GameEvent.Register(new Events.Player.Inventory.PickupEvent(carriable));
        }

        [ClientRpc]
        public static void ClientOnPlayerCarriableItemDrop(Entity carriable)
        {
            GameEvent.Register(new Events.Player.Inventory.DropEvent(carriable));

            // TODO remove ClientRPCs with events.RunNetworked
        }

        [ClientRpc]
        public static void ClientClearInventory()
        {
            GameEvent.Register(new Events.Player.Inventory.ClearEvent());
        }

        [ClientRpc]
        public static void ClientDisplayMessage(string message, Color color)
        {
            InfoFeed.Current?.AddEntry(message, color);
        }
    }
}
