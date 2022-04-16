using System;
using System.Collections.Generic;
using System.IO;

using Sandbox;

namespace TTTReborn.Events.Game
{
    [GameEvent("game_gameresult"), Hammer.Skip]
    public partial class GameResultsEvent : NetworkableGameEvent
    {
        public List<ILoggedGameEvent> GameEvents { get; set; } = new();

        /// <summary>
        /// Called when events and score was processed by the server.
        /// </summary>
        public GameResultsEvent(List<ILoggedGameEvent> gameEvents) : base()
        {
            GameEvents = gameEvents ?? new();
        }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public GameResultsEvent() : base() { }

        public override void Run() => Event.Run(Name, GameEvents);

        public override void WriteData(BinaryWriter binaryWriter)
        {
            base.WriteData(binaryWriter);

            binaryWriter.Write(GameEvents.Count);

            foreach (GameEvent gameEvent in GameEvents)
            {
                if (gameEvent is not NetworkableGameEvent networkableGameEvent)
                {
                    continue;
                }

                binaryWriter.Write(gameEvent.Name);
                networkableGameEvent.WriteData(binaryWriter);
            }
        }

        public override void ReadData(BinaryReader binaryReader)
        {
            base.ReadData(binaryReader);

            GameEvents.Clear();

            int count = binaryReader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                Type type = Utils.GetTypeByLibraryName<ILoggedGameEvent>(binaryReader.ReadString());

                if (type == null)
                {
                    continue;
                }

                ILoggedGameEvent loggedGameEvent = Utils.GetNetworkableObjectByType<ILoggedGameEvent>(type, binaryReader);

                if (!type.IsSubclassOf(typeof(NetworkableGameEvent)))
                {
                    continue;
                }

                GameEvents.Add(loggedGameEvent);
            }
        }
    }
}
