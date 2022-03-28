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

        public virtual void Run() => Event.Run(Name);

        public virtual void RunNetworked(To to)
        {
            Run();

            if (Host.IsServer)
            {
                ClientRun(to, this);
            }
        }

        public void RunNetworked() => RunNetworked(To.Everyone);

        [ClientRpc]
        public static void ClientRun(GameEvent gameEvent)
        {
            gameEvent.Run();
        }
    }
}
