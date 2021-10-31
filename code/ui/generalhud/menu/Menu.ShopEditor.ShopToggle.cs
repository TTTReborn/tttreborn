using System;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private static bool ProcessShopToggle(string roleName, bool toggle)
        {
            Type roleType = Utils.GetTypeByLibraryName<TTTRole>(roleName);

            if (roleType == null)
            {
                return false;
            }

            TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

            if (role == null)
            {
                return false;
            }

            role.Shop.Enabled = toggle;

            if (Host.IsServer)
            {
                foreach (Client client in Client.All)
                {
                    if (client.Pawn is TTTPlayer player && player.Role.Equals(roleName))
                    {
                        player.Shop.Enabled = toggle;
                    }
                }
            }
            else if (Local.Client?.Pawn is TTTPlayer player && player.Role.Name.Equals(roleName))
            {
                player.Shop.Enabled = toggle;
            }

            return true;
        }

        [ServerCmd]
        public static void ServerToggleShop(string roleName, bool toggle)
        {
            if (!(ConsoleSystem.Caller?.HasPermission("shopeditor") ?? false))
            {
                return;
            }

            if (ProcessShopToggle(roleName, toggle))
            {
                Shop.Save(Utils.GetObjectByType<TTTRole>(Utils.GetTypeByLibraryName<TTTRole>(roleName)));

                ClientToggleShop(roleName, toggle);
            }
        }

        [ClientRpc]
        public static void ClientToggleShop(string roleName, bool toggle)
        {
            if (ProcessShopToggle(roleName, toggle))
            {
                Menu menu = Menu.Instance;

                if (menu == null || !menu.Enabled)
                {
                    return;
                }

                PanelContent menuContent = menu.Content;

                if (menuContent == null || !menuContent.ClassName.Equals("shopeditor") || !roleName.Equals(menu._selectedRole?.Name) || menu._shopToggle == null)
                {
                    return;
                }

                menu._shopToggle.Checked = toggle;
            }
        }
    }
}
