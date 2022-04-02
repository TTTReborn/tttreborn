using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Sandbox;

using TTTReborn.Globalization;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_takedamage"), Hammer.Skip]
    public partial class TakeDamageEvent : PlayerGameEvent, ILoggedGameEvent
    {
        public TranslationData GetDescriptionTranslationData() => new(GetTranslationKey("DESCRIPTION"), PlayerName ?? "???", Damage, AttackerName ?? "???");

        public float Damage { get; set; }

        public int AttackerIdent { get; set; }

        [JsonIgnore]
        public Entity Attacker
        {
            get => Entity.All.First((ent) => ent.NetworkIdent == AttackerIdent);
        }

        public string AttackerName { get; set; }

        public bool IsAttackerPlayer { get; set; }

        /// <summary>
        /// Occurs when a player takes damage.
        /// <para>The <strong><see cref="TTTReborn.Player"/></strong> instance of the player who took damage.</para>
        /// <para>The <strong><see cref="float"/></strong> of the amount of damage taken.</para>
        /// <para>The <strong><see cref="Entity"/></strong> that applied the damage.</para>
        /// </summary>
        public TakeDamageEvent(TTTReborn.Player player, float damage, Entity attacker) : base(player)
        {
            Damage = damage;

            if (player != null && attacker != null)
            {
                IsAttackerPlayer = attacker is TTTReborn.Player;
                AttackerIdent = attacker.NetworkIdent;

                if (IsAttackerPlayer)
                {
                    AttackerName = attacker.Client.Name;
                }
                else
                {
                    AttackerName = attacker.Name;
                }
            }
        }

        public override void Run() => Event.Run(Name, Player, Damage);

        [Event(typeof(Game.LoggedGameEventEvaluateEvent))]
        public static void OnLoggedGameEventEvaluate(List<ILoggedGameEvent> gameEvents)
        {
            // merge same events with same attacker together
            List<ILoggedGameEvent> copy = new(gameEvents);

            gameEvents.Clear();

            TakeDamageEvent current = null;

            foreach (ILoggedGameEvent gameEvent in copy)
            {
                if (gameEvent is TakeDamageEvent takeDamageEvent)
                {
                    if (current == null)
                    {
                        current = takeDamageEvent;
                    }
                    else if (current.Ident == takeDamageEvent.Ident && current.AttackerIdent == takeDamageEvent.AttackerIdent)
                    {
                        current.Damage += takeDamageEvent.Damage;
                    }
                    else
                    {
                        gameEvents.Add(current);

                        current = takeDamageEvent;
                    }
                }
                else
                {
                    if (current != null)
                    {
                        gameEvents.Add(current);

                        current = null;
                    }

                    gameEvents.Add(gameEvent);
                }
            }
        }
    }
}
