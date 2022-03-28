using System;

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

    public abstract partial class GameEvent : BaseNetworkable
    {
        public string Name { get; }

        public float CreatedAt { get; }

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

        protected abstract void ServerCallNetworked(To to);

        // Due to sbox [Net] assignment and aquisition issues, this is not possible currently.
        // So instead of doing it with a single function, we have to implement networking per GameEvent
        // [ClientRpc]
        // public static void ClientRun(GameEvent gameEvent)
        // {
        //     gameEvent.Run();
        // }

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
