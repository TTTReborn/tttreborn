using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn
{
    public abstract partial class NetworkableGameEvent : GameEvent
    {
        [JsonIgnore]
        public To? Receiver { get; set; } = null;

        public void RunNetworked() => RunNetworked(Receiver ?? To.Everyone);

        public virtual void RunNetworked(To to)
        {
            Receiver = to;

            base.Run();

            if (Game.IsServer)
            {
                ServerCallNetworked(to);
            }
        }

        protected virtual string[] GetJsonData() => Array.Empty<string>();

        protected virtual void Init(string[] jsonData) { }

        protected virtual void ServerCallNetworked(To to)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = false
            };

            ClientRun(to, Name, JsonSerializer.Serialize(this, GetType(), options), GetJsonData() ?? Array.Empty<string>());
        }

        [ClientRpc]
        public static void ClientRun(string libraryName, string jsonEventData, string[] jsonData)
        {
            Type type = Utils.GetTypeByLibraryName<GameEvent>(libraryName);

            if (type == null)
            {
                return;
            }

            NetworkableGameEvent gameEvent = JsonSerializer.Deserialize(jsonEventData, type) as NetworkableGameEvent;
            gameEvent?.Init(jsonData);
            gameEvent?.Run();
        }

        public static void RegisterNetworked<T>(T gameEvent, params GameEventScoring[] gameEventScorings) where T : NetworkableGameEvent => RegisterNetworked(To.Everyone, gameEvent, gameEventScorings);

        public static void RegisterNetworked<T>(To to, T gameEvent, params GameEventScoring[] gameEventScorings) where T : NetworkableGameEvent
        {
            gameEvent.Scoring = gameEventScorings ?? gameEvent.Scoring;

            gameEvent.ProcessRegister();
            gameEvent.RunNetworked(to);
        }
    }
}
