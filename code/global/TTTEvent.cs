using Sandbox;

namespace TTTReborn.Globals
{
    public static class TTTEvent
    {
        public static class Player
        {
            public const string Died = "tttreborn.player.died";
            public const string InitialSpawn = "tttreborn.player.initialspawn";
            public const string Spawned = "tttreborn.player.spawned";
            public const string TakeDamage = "tttreborn.player.takedamage";

            public static class CarriableItem
            {
                public const string Drop = "tttreborn.player.carriableitem.drop";
                public const string PickUp = "tttreborn.player.carriableitem.pickup";
            }

            public static class Inventory
            {
                public const string Clear = "tttreborn.player.inventory.clear";
            }

            public static class Role
            {
                public const string OnSelect = "tttreborn.player.role.onselect";
            }

            public static class Spectating
            {
                public const string Change = "tttreborn.player.spectating.change";
            }
        }

        public static class Settings
        {
            public static class Instance
            {
                public const string Change = "tttreborn.settings.instance.change";
            }
        }

        public static class Shop
        {
            public const string Change = "tttreborn.shop.change";
        }
    }
}
