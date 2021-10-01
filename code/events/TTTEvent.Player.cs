namespace TTTReborn.Events
{
    public static partial class TTTEvent
    {
        public static class Player
        {
            /// <summary>
            /// Occurs when a player dies.
            /// <para>Event is passed the <strong><see cref="TTTReborn.Player.TTTPlayer"/></strong> instance of the player who died.</para>
            /// </summary>
            public const string Died = "tttreborn.player.died";

            /// <summary>
            /// Occurs when a player initializes.
            /// <para>No data is passed to this event.</para>
            /// </summary>
            public const string InitialSpawn = "tttreborn.player.initialspawn";

            /// <summary>
            /// Occurs when a player spawns.
            /// <para>Event is passed the <strong><see cref="TTTReborn.Player.TTTPlayer"/></strong> instance of the player spawned.</para>
            /// </summary>
            public const string Spawned = "tttreborn.player.spawned";

            /// <summary>
            /// Occurs when a player takes damage.
            /// <para>The <strong><see cref="TTTReborn.Player.TTTPlayer"/></strong> instance of the player who took damage.</para>
            /// <para>The <strong><see cref="float"/></strong> of the amount of damage taken.</para>
            /// </summary>
            public const string TakeDamage = "tttreborn.player.takedamage";

            public static class Inventory
            {
                /// <summary>
                /// Occurs when the player's inventory is cleared.
                /// <para>No data is passed to this event.</para>
                /// </summary>
                public const string Clear = "tttreborn.player.inventory.clear";

                /// <summary>
                /// Occurs when an item is dropped.
                /// <para>Event is passed the <strong><see cref="Items.ICarriableItem"/></strong> instance of the item dropped.</para>
                /// </summary>
                public const string Drop = "tttreborn.player.inventory.drop";

                /// <summary>
                /// Occurs when an item is picked up.
                /// <para>Event is passed the <strong><see cref="Items.ICarriableItem"/></strong> instance of the item picked up.</para>
                /// </summary>
                public const string PickUp = "tttreborn.player.inventory.pickup";
            }

            public static class Role
            {
                /// <summary>
                /// Occurs when a player selects their role.
                /// <para>Event is passed the <strong><see cref="TTTReborn.Player.TTTPlayer"/></strong> instance of the player whose role was set.</para>
                /// </summary>
                public const string Select = "tttreborn.player.role.select";
            }

            public static class Spectating
            {
                /// <summary>
                /// Occurs when the player is changed to spectate.
                /// <para>Event is passed the <strong><see cref="TTTReborn.Player.TTTPlayer"/></strong> instance of the player who was changed to spectate.</para>
                /// </summary>
                public const string Change = "tttreborn.player.spectating.change";
            }
        }
    }
}
