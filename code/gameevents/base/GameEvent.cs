using System;
using System.Text.Json;

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

            (JsonSerializer.Deserialize(json, type) as GameEvent)?.Run();
        }

        protected virtual void OnRegister() { }

        public static void Register<T>(T gameEvent, bool isNetworked = false) where T : GameEvent
        {
            gameEvent.OnRegister();

            if (isNetworked)
            {
                gameEvent.RunNetworked();
            }
            else
            {
                gameEvent.Run();
            }
        }

        public static void Register<T>(T gameEvent, To to) where T : GameEvent
        {
            gameEvent.OnRegister();
            gameEvent.RunNetworked(to);
        }
    }
}
