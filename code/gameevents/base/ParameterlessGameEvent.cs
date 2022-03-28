using Sandbox;

namespace TTTReborn.Events
{
    public partial class ParameterlessGameEvent : GameEvent
    {
        public ParameterlessGameEvent() : base() { }

        protected override void ServerCallNetworked(To to) => ClientRun(to, this);

        [ClientRpc]
        public static void ClientRun(GameEvent gameEvent)
        {
            gameEvent.Run();
        }
    }
}
