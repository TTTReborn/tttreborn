// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Items;
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
        public static void ClientOnPlayerDisconnect(long playerId, NetworkDisconnectionReason reason)
        {
            Event.Run(TTTEvent.Player.Disconnected, playerId, reason);
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

            player.SetRole(Utils.GetObjectByType<TTTRole>(Utils.GetTypeByLibraryName<TTTRole>(roleName)), TeamFunctions.GetTeam(teamName));

            Client client = player.Client;

            if (client == null || !client.IsValid())
            {
                return;
            }

            Scoreboard.Instance?.UpdateClient(client);
        }

        [ClientRpc]
        public static void ClientConfirmPlayer(TTTPlayer confirmPlayer, PlayerCorpse playerCorpse, TTTPlayer deadPlayer, string roleName, string teamName, ConfirmationData confirmationData, string killerWeapon, string[] perks)
        {
            if (!deadPlayer.IsValid())
            {
                return;
            }

            deadPlayer.SetRole(Utils.GetObjectByType<TTTRole>(Utils.GetTypeByLibraryName<TTTRole>(roleName)), TeamFunctions.GetTeam(teamName));

            deadPlayer.IsConfirmed = true;
            deadPlayer.CorpseConfirmer = confirmPlayer;

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
        public static void ClientOpenMapSelectionMenu()
        {
            MapSelectionMenu.Instance.Enabled = true;
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
