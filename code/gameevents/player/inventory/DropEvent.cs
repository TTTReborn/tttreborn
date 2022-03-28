using Sandbox;

namespace TTTReborn.Events.Player.Inventory
{
    [GameEvent("player_inventory_drop")]
    public partial class DropEvent : GameEvent
    {
        public Entity Entity { get; set; }

        /// <summary>
        /// Occurs when an item is dropped.
        /// <para>Event is passed the <strong><see cref="Sandbox.Entity"/></strong> instance of the item dropped.</para>
        /// </summary>
        public DropEvent(Entity entity) : base()
        {
            Entity = entity;
        }

        public override void Run() => Event.Run(Name, Entity);

        protected override void ServerCallNetworked(To to) => ClientRun(to, this, Entity);

        [ClientRpc]
        public static void ClientRun(DropEvent gameEvent, Entity entity)
        {
            gameEvent.Entity = entity;
            gameEvent.Run();
        }
    }
}
