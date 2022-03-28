using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class EntityGameEvent : GameEvent
    {
        public int Ident { get; set; }

        [JsonIgnore]
        public Entity Entity
        {
            get => Entity.All.First((ent) => ent.NetworkIdent == Ident);
        }

        public EntityGameEvent(Entity entity) : base()
        {
            if (entity != null)
            {
                Ident = entity.NetworkIdent;
            }
        }

        public override void Run() => Event.Run(Name, Entity);

        protected override void ServerCallNetworked(To to) => ClientRun(to, JsonSerializer.Serialize(this));

        [ClientRpc]
        public static void ClientRun(string json)
        {
            Dezerialize<EntityGameEvent>(json)?.Run();
        }
    }
}
