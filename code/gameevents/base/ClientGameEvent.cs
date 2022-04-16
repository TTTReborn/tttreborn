using System.IO;
using System.Linq;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class ClientGameEvent : NetworkableGameEvent
    {
        public long PlayerId { get; set; }

        public string PlayerName { get; set; }

        public Client Client
        {
            get => Client.All.First((cl) => cl.PlayerId == PlayerId);
        }

        public ClientGameEvent(Client client) : base()
        {
            PlayerId = client.PlayerId;
            PlayerName = client.Name;
        }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public ClientGameEvent() : base() { }

        public override void Run() => Event.Run(Name, Client);

        public override void WriteData(BinaryWriter binaryWriter)
        {
            base.WriteData(binaryWriter);

            binaryWriter.Write(PlayerId);
            binaryWriter.Write(PlayerName);
        }

        public override void ReadData(BinaryReader binaryReader)
        {
            base.ReadData(binaryReader);

            PlayerId = binaryReader.ReadInt64();
            PlayerName = binaryReader.ReadString();
        }
    }
}
