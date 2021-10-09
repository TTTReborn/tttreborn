using System.Text.Json;

using Sandbox;

using TTTReborn.Roles;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private static void EditItem(QuickShopItem item, TTTRole role)
        {
            if (!item.HasClass("selected"))
            {
                return;
            }

            Log.Error($"Edit {item.ItemData.Name}");

            // ServerUpdateItem(item.ItemData.Name, true, JsonSerializer.Serialize(item.ItemData), role.Name);
        }
    }
}
