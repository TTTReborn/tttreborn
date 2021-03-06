using System;
using System.Text.Json;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public partial class ShopEditorPage : Panel
    {
        [ConCmd.Server]
        public static void ServerRequestShopEditorAccess()
        {
            if (ConsoleSystem.Caller == null)
            {
                return;
            }

            To to = To.Single(ConsoleSystem.Caller);

            if (!ConsoleSystem.Caller.HasPermission("shopeditor"))
            {
                ClientReceiveShopEditorAccess(to, false);

                return;
            }

            foreach (Type roleType in Utils.GetTypes<Role>())
            {
                Role role = Utils.GetObjectByType<Role>(roleType);

                ClientUpdateRoleShop(to, role.Name, JsonSerializer.Serialize(role.Shop));
            }

            ClientReceiveShopEditorAccess(to, true);
        }

        [ClientRpc]
        public static void ClientUpdateRoleShop(string roleName, string shopJson)
        {
            Type roleType = Utils.GetTypeByLibraryName<Role>(roleName);

            if (roleType == null)
            {
                return;
            }

            Role role = Utils.GetObjectByType<Role>(roleType);

            if (role == null)
            {
                return;
            }

            role.Shop = Shop.InitializeFromJSON(shopJson);
        }

        [ClientRpc]
        public static void ClientReceiveShopEditorAccess(bool _)
        {
            TTTMenu.Instance.AddPage(new ShopEditorPage());
        }

        [ConCmd.Server]
        public static void ServerToggleShop(string roleName, bool toggle)
        {
            if (!(ConsoleSystem.Caller?.HasPermission("shopeditor") ?? false))
            {
                return;
            }

            if (ProcessShopToggle(roleName, toggle))
            {
                Shop.Save(Utils.GetObjectByType<Role>(Utils.GetTypeByLibraryName<Role>(roleName)));

                ClientToggleShop(roleName, toggle);
            }
        }

        [ClientRpc]
        public static void ClientToggleShop(string roleName, bool toggle)
        {
            ProcessShopToggle(roleName, toggle);
        }

        private static bool ProcessShopToggle(string roleName, bool toggle)
        {
            Type roleType = Utils.GetTypeByLibraryName<Role>(roleName);

            if (roleType == null)
            {
                return false;
            }

            Role role = Utils.GetObjectByType<Role>(roleType);

            if (role == null)
            {
                return false;
            }

            role.Shop.Enabled = toggle;

            if (Host.IsServer)
            {
                foreach (Client client in Client.All)
                {
                    if (client.Pawn is Player player && player.Role.Equals(roleName))
                    {
                        player.Shop.Enabled = toggle;
                    }
                }
            }
            else if (Local.Client?.Pawn is Player player && player.Role.Name.Equals(roleName))
            {
                player.Shop.Enabled = toggle;
            }

            return true;
        }
    }
}
