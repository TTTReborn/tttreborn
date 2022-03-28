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

    public partial class GameEvent
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
            else
            {
                Log.Warning($"'{GetType()}' is missing GameEventAttribute");
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

        protected static T Dezerialize<T>(string json) where T : GameEvent
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
            {
                WriteIndented = false
            });
        }

        protected virtual void ServerCallNetworked(To to) => ClientRun(to, JsonSerializer.Serialize(this));

        [ClientRpc]
        public static void ClientRun(string json)
        {
            Dezerialize<GameEvent>(json)?.Run();
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
    }
}
