using System;

using Sandbox;

namespace TTTReborn.Events
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GameEventAttribute : LibraryAttribute
    {
        public GameEventAttribute(string name) : base($"ttt_gameevent_{name}".ToLower()) { }
    }

    public class TTTEventAttribute : Sandbox.EventAttribute
    {
        public TTTEventAttribute(Type type) : base(Utils.GetAttribute<GameEventAttribute>(type).Name) { }
    }

    public abstract partial class GameEvent : BaseNetworkable
    {
        [Net]
        public string Name { get; }

        [Net]
        public float CreatedAt { get; }

        [Net]
        public int Score { get; set; }

        [Net]
        public Player Owner { get; set; }

        public GameEvent() : base()
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

        public virtual object[] GetParameters()
        {
            return Array.Empty<object>();
        }

        public static void Run(GameEvent gameEvent) => Event.Run(gameEvent.Name, gameEvent.GetParameters());

        public static void RunNetworked(To to, GameEvent gameEvent)
        {
            Run(gameEvent);

            if (Host.IsServer)
            {
                ClientRun(to, gameEvent);
            }
        }

        public static void RunNetworked(GameEvent gameEvent) => RunNetworked(To.Everyone, gameEvent);

        [ClientRpc]
        public static void ClientRun(GameEvent gameEvent)
        {
            Run(gameEvent);
        }
    }
}
