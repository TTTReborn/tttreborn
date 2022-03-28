using Sandbox;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_disconnected")]
    public partial class DisconnectedEvent : GameEvent
    {
        public long PlayerId { get; set; }

        public NetworkDisconnectionReason Reason { get; set; }

        /// <summary>
        /// Occurs when a player disconnects.
        /// <para>The <strong><see cref="long"/></strong> of the player's PlayerId who disconnected.</para>
        /// <para>The <strong><see cref="NetworkDisconnectionReason"/></strong>.</para>
        /// </summary>
        public DisconnectedEvent(long playerId, NetworkDisconnectionReason reason) : base()
        {
            PlayerId = playerId;
            Reason = reason;
        }

        public override void Run() => Event.Run(Name, PlayerId, Reason);

        protected override void ServerCallNetworked(To to) => ClientRun(to, this, PlayerId, Reason);

        [ClientRpc]
        public static void ClientRun(DisconnectedEvent gameEvent, long playerId, NetworkDisconnectionReason reason)
        {
            gameEvent.PlayerId = playerId;
            gameEvent.Reason = reason;
            gameEvent.Run();
        }
    }
}
