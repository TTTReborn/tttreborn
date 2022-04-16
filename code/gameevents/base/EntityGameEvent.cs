using System.IO;
using System.Linq;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class EntityGameEvent : NetworkableGameEvent
    {
        public int Ident { get; set; }

        public Entity Entity
        {
            get => Entity.All.First((ent) => ent.NetworkIdent == Ident);
        }

        public EntityGameEvent(Entity entity) : base()
        {
            Ident = entity.NetworkIdent;
        }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public EntityGameEvent() : base() { }

        public override void Run() => Event.Run(Name, Entity);

        public override void WriteData(BinaryWriter binaryWriter)
        {
            base.WriteData(binaryWriter);

            binaryWriter.Write(Ident);
        }

        public override void ReadData(BinaryReader binaryReader)
        {
            base.ReadData(binaryReader);

            Ident = binaryReader.ReadInt32();
        }
    }
}
