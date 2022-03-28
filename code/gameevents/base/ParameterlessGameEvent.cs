using System.Text.Json;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class ParameterlessGameEvent : GameEvent
    {
        public ParameterlessGameEvent() : base() { }

        public override void Run() => Event.Run(Name);

        protected override void ServerCallNetworked(To to) => ClientRun(to, JsonSerializer.Serialize(this));

        [ClientRpc]
        public static void ClientRun(string json)
        {
            Dezerialize<ParameterlessGameEvent>(json)?.Run();
        }
    }
}
