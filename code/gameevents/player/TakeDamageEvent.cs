using System.Text.Json;

using Sandbox;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_takedamage")]
    public partial class TakeDamageEvent : PlayerGameEvent
    {
        public float Damage { get; set; }

        /// <summary>
        /// Occurs when a player takes damage.
        /// <para>The <strong><see cref="TTTReborn.Player"/></strong> instance of the player who took damage.</para>
        /// <para>The <strong><see cref="float"/></strong> of the amount of damage taken.</para>
        /// </summary>
        public TakeDamageEvent(TTTReborn.Player player, float damage) : base(player)
        {
            Damage = damage;
        }

        public override void Run() => Event.Run(Name, Player, Damage);

        protected override void ServerCallNetworked(To to) => ClientRun(to, JsonSerializer.Serialize(this));

        [ClientRpc]
        public new static void ClientRun(string json)
        {
            Dezerialize<TakeDamageEvent>(json)?.Run();
        }
    }
}
