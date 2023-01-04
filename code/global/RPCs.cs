using Sandbox;

using TTTReborn.Roles;
using TTTReborn.Teams;
using TTTReborn.UI;

namespace TTTReborn.Globals
{
    public partial class RPCs
    {
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

            IClient client = player.Client;

            if (client == null || !client.IsValid())
            {
                return;
            }

            Scoreboard.Instance?.UpdateClient(client);
        }

        [ClientRpc]
        public static void ClientConfirmPlayer(Player confirmPlayer, PlayerCorpse playerCorpse, string jsonData, bool covert = false)
        {
            ConfirmationData data = PlayerCorpse.GetDezerializedData(jsonData);

            if (data == null)
            {
                return;
            }

            Player deadPlayer = data.Player;

            if (deadPlayer.IsValid())
            {
                deadPlayer.SetRole(Utils.GetObjectByType<Role>(Utils.GetTypeByLibraryName<Role>(data.RoleName)), TeamFunctions.GetTeam(data.TeamName));

                if (!covert)
                {
                    deadPlayer.IsConfirmed = true;
                    deadPlayer.CorpseConfirmer = confirmPlayer;
                }
            }

            if (playerCorpse.IsValid())
            {
                playerCorpse.DeadPlayer = deadPlayer;
                playerCorpse.Data = data;

                InspectMenu.Instance.SetPlayerData(playerCorpse);
            }

            if (deadPlayer.IsValid())
            {
                Scoreboard.Instance.UpdateClient(deadPlayer.Client);
            }

            if (!confirmPlayer.IsValid())
            {
                return;
            }

            IClient confirmClient = confirmPlayer.Client;

            // TODO improve
            if (deadPlayer.IsValid())
            {
                InfoFeed.Current?.AddEntry(
                    confirmClient,
                    deadPlayer.Client,
                    "found the body of",
                    $"({deadPlayer.Role.Name})"
                );
            }
            else
            {
                InfoFeed.Current?.AddEntry(
                    confirmClient,
                    $"found the body of {data.Name}!"
                );
            }

            if (confirmPlayer == Game.LocalPawn as Player && data.Credits > 0)
            {
                InfoFeed.Current?.AddEntry(
                    confirmClient,
                    $"found $ {data.Credits} credits!"
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
        public static void ClientDisplayMessage(string message, Color color)
        {
            InfoFeed.Current?.AddEntry(message, color);
        }
    }
}
