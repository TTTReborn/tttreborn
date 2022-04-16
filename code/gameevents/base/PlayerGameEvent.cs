using System.IO;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class PlayerGameEvent : ClientGameEvent
    {
        public TTTReborn.Player Player
        {
            get => Utils.GetPlayerById(PlayerId);
        }

        public PlayerGameEvent(TTTReborn.Player player) : base(player?.Client) { }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public PlayerGameEvent() : base() { }

        public override void Run() => Event.Run(Name, Player);
    }
}
