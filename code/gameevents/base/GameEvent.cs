using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GameEventAttribute : LibraryAttribute
    {
        public GameEventAttribute(string name) : base($"ttt_gameevent_{name}".ToLower()) { }
    }

    public class EventAttribute : Sandbox.EventAttribute
    {
        public EventAttribute(Type type) : base(Utils.GetAttribute<GameEventAttribute>(type).Name) { }
    }

    public abstract partial class GameEvent
    {
        public string Name { get; set; }

        public float CreatedAt { get; set; }

        [JsonIgnore]
        public GameEventScoring[] Scoring { get; set; } = Array.Empty<GameEventScoring>();

        public GameEvent()
        {
            GameEventAttribute attribute = Utils.GetAttribute<GameEventAttribute>(GetType());

            if (attribute != null)
            {
                Name = attribute.Name;
            }

            CreatedAt = Time.Now;
        }

        public virtual void Run() => Event.Run(Name);

        public void RunNetworked() => RunNetworked(To.Everyone);

        public virtual void RunNetworked(To to)
        {
            Run();

            if (Host.IsServer)
            {
                ServerCallNetworked(to);
            }
        }

        protected virtual void ServerCallNetworked(To to) => ClientRun(to, Name, JsonSerializer.Serialize(this, GetType(), new JsonSerializerOptions()
        {
            WriteIndented = false
        }));

        [ClientRpc]
        public static void ClientRun(string libraryName, string json)
        {
            Type type = Utils.GetTypeByLibraryName<GameEvent>(libraryName);

            if (type == null)
            {
                return;
            }

            Log.Debug($"Calling '{type}'");

            (JsonSerializer.Deserialize(json, type) as GameEvent)?.Run();
        }

        protected virtual void OnRegister()
        {
            foreach (GameEventScoring gameEventScoring in Scoring)
            {
                gameEventScoring.Init(this);
            }
        }

        private void ProcessRegister()
        {
            if (Host.IsServer)
            {
                Gamemode.Game.Instance.Round?.GameEvents.Add(this);

                OnRegister();
            }
        }

        public static void Register<T>(T gameEvent, params GameEventScoring[] gameEventScorings) where T : GameEvent
        {
            gameEvent.Scoring = gameEventScorings ?? gameEvent.Scoring;

            gameEvent.ProcessRegister();
            gameEvent.Run();
        }

        public static void RegisterNetworked<T>(T gameEvent, params GameEventScoring[] gameEventScorings) where T : GameEvent => RegisterNetworked(To.Everyone, gameEvent, gameEventScorings);

        public static void RegisterNetworked<T>(To to, T gameEvent, params GameEventScoring[] gameEventScorings) where T : GameEvent
        {
            gameEvent.Scoring = gameEventScorings ?? gameEvent.Scoring;

            gameEvent.ProcessRegister();
            gameEvent.RunNetworked(to);
        }
    }

    public partial class GameEventScoring
    {
        public int Score { get; set; } = 0;
        public int Karma { get; set; } = 0;
        public Player Player { get; set; }

        public bool IsInitialized { get; set; } = false;

        public virtual void Init<T>(T gameEvent) where T : GameEvent
        {
            IsInitialized = true;
        }

        public virtual void Evaluate()
        {
            if (Player != null && Player.IsValid)
            {
                Player.Client.SetInt("score", Player.Client.GetInt("score") + Score);
                Player.Client.SetInt("karma", Player.Client.GetInt("karma") + Karma);
            }
        }

        public GameEventScoring(Player player)
        {
            Player = player;
        }
    }
}
