using System.IO;

using Sandbox;

using TTTReborn.Globalization;

namespace TTTReborn.Events.Player.Role
{
    [GameEvent("player_role_select"), Hammer.Skip]
    public partial class SelectEvent : PlayerGameEvent, ILoggedGameEvent
    {
        public string RoleName { get; set; }

        public TranslationData GetDescriptionTranslationData() => new(GetTranslationKey("DESCRIPTION"), PlayerName ?? "???", RoleName != null ? new TranslationData(Utils.GetTranslationKey(RoleName, "NAME")) : "???");

        /// <summary>
        /// Occurs when a player selects their role.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player whose role was set.</para>
        /// </summary>
        public SelectEvent(TTTReborn.Player player) : base(player)
        {
            RoleName = player.Role.Name;
        }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public SelectEvent() : base() { }

        public bool Contains(Client client) => PlayerName == client.Name;

        public override void WriteData(BinaryWriter binaryWriter)
        {
            base.WriteData(binaryWriter);

            binaryWriter.Write(RoleName);
        }

        public override void ReadData(BinaryReader binaryReader)
        {
            base.ReadData(binaryReader);

            RoleName = binaryReader.ReadString();
        }
    }
}
