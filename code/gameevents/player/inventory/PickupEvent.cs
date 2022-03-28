using Sandbox;

namespace TTTReborn.Events.Player.Inventory
{
    [GameEvent("player_inventory_pickup")]
    public partial class PickupEvent : GameEvent
    {
        public Entity Entity { get; set; }

        /// <summary>
        /// Occurs when an item is picked up.
        /// <para>Event is passed the <strong><see cref="Sandbox.Entity"/></strong> instance of the item picked up.</para>
        /// </summary>
        public PickupEvent(Entity entity) : base()
        {
            Entity = entity;
        }

        public override void Run() => Event.Run(Name, Entity);

        protected override void ServerCallNetworked(To to) => ClientRun(to, this, Entity);

        [ClientRpc]
        public static void ClientRun(PickupEvent gameEvent, Entity entity)
        {
            gameEvent.Entity = entity;
            gameEvent.Run();
        }
    }
}
