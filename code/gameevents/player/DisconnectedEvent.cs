using System.IO;

using Sandbox;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_disconnected"), Hammer.Skip]
    public partial class DisconnectedEvent : NetworkableGameEvent
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

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public DisconnectedEvent() : base() { }

        public override void Run() => Event.Run(Name, PlayerId, Reason);

        public override void WriteData(BinaryWriter binaryWriter)
        {
            base.WriteData(binaryWriter);

            binaryWriter.Write(PlayerId);
            binaryWriter.Write((int) Reason);
        }

        public override void ReadData(BinaryReader binaryReader)
        {
            base.ReadData(binaryReader);

            PlayerId = binaryReader.ReadInt64();
            Reason = (NetworkDisconnectionReason) binaryReader.ReadInt32();
        }
    }
}
