using Sandbox;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_takedamage")]
    public partial class TakeDamageEvent : PlayerGameEvent, ILoggedGameEvent
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
    }
}
