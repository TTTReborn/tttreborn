using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn.Events.Game
{
    public struct GameResultData
    {
        public string Name { get; set; }
        public string Data { get; set; }

        public GameResultData(string name, string data)
        {
            Name = name;
            Data = data;
        }
    }

    [GameEvent("game_gameresult")]
    public partial class GameResultsEvent : GameEvent
    {
        [JsonIgnore]
        public List<GameEvent> GameEvents { get; set; } = new();

        /// <summary>
        /// Called when events and score was processed by the server.
        /// </summary>
        public GameResultsEvent(List<GameEvent> gameEvents) : base()
        {
            GameEvents = gameEvents ?? new();
        }

        public override void Run() => Event.Run(Name, GameEvents);

        protected override string[] GetJsonData()
        {
            List<GameResultData> gameEventsDict = new();

            JsonSerializerOptions options = new()
            {
                WriteIndented = false
            };

            foreach (GameEvent gameEvent in GameEvents)
            {
                if (gameEvent is RoundChangeEvent) // TODO replace with check for NetworkableGameEvent
                {
                    continue;
                }

                gameEventsDict.Add(new(gameEvent.Name, JsonSerializer.Serialize(gameEvent, gameEvent.GetType(), options)));
            }

            return new string[]
            {
                JsonSerializer.Serialize(gameEventsDict, options)
            };
        }

        protected override void Init(string[] jsonData)
        {
            base.Init(jsonData);

            foreach (GameResultData gameResultData in JsonSerializer.Deserialize<List<GameResultData>>(jsonData[0]))
            {
                Type gameEventType = Utils.GetTypeByLibraryName<GameEvent>(gameResultData.Name);

                if (gameEventType == null || gameEventType == typeof(RoundChangeEvent)) // TODO replace with check for NetworkableGameEvent
                {
                    continue;
                }

                GameEvents.Add(JsonSerializer.Deserialize(gameResultData.Data, gameEventType) as GameEvent);
            }
        }
    }
}
