using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.Teams;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_takedamage"), Hammer.Skip]
    public partial class TakeDamageEvent : PlayerGameEvent, ILoggedGameEvent
    {
        public TranslationData GetDescriptionTranslationData() => new(GetTranslationKey("DESCRIPTION"), PlayerName ?? "???", Damage, AttackerName ?? "???");

        public float Damage { get; set; }

        public int AttackerIdent { get; set; }

        public string AttackerName { get; set; }

        public long? AttackerPlayerId { get; set; }

        public Entity Attacker
        {
            get
            {
                if (AttackerPlayerId != null)
                {
                    return Utils.GetPlayerById((long) AttackerPlayerId);
                }

                return Entity.All.First((ent) => ent.NetworkIdent == AttackerIdent);
            }
        }

        /// <summary>
        /// Occurs when a player takes damage.
        /// <para>The <strong><see cref="TTTReborn.Player"/></strong> instance of the player who took damage.</para>
        /// <para>The <strong><see cref="float"/></strong> of the amount of damage taken.</para>
        /// <para>The <strong><see cref="Entity"/></strong> that applied the damage.</para>
        /// </summary>
        public TakeDamageEvent(TTTReborn.Player player, float damage, Entity attacker) : base(player)
        {
            Damage = damage;

            if (attacker != null)
            {
                AttackerIdent = attacker.NetworkIdent;

                if (attacker is TTTReborn.Player)
                {
                    AttackerName = attacker.Client.Name;
                    AttackerPlayerId = attacker.Client.PlayerId;
                }
                else
                {
                    AttackerName = attacker.Name;
                    AttackerPlayerId = null;
                }
            }
        }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public TakeDamageEvent() : base() { }

        public override void Run() => Event.Run(Name, Player, Damage);

        [Event(typeof(Game.LoggedGameEventsEvaluateEvent))]
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
                    else if (current.PlayerId == takeDamageEvent.PlayerId && current.AttackerPlayerId == takeDamageEvent.AttackerPlayerId)
                    {
                        current.Damage += takeDamageEvent.Damage;

                        foreach (GameEventScoring scoring in takeDamageEvent.Scoring)
                        {
                            foreach (GameEventScoring currentScoring in current.Scoring)
                            {
                                if (scoring.PlayerId == currentScoring.PlayerId)
                                {
                                    currentScoring.Score += scoring.Score;
                                    currentScoring.Karma += scoring.Karma;
                                }
                            }
                        }
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

        public bool Contains(Client client) => PlayerName == client.Name || AttackerName == client.Name;

        protected override void OnRegister()
        {
            if (Player == null || !Player.IsValid || Attacker is not TTTReborn.Player attacker || !Attacker.IsValid)
            {
                return;
            }

            if (attacker.IsTeamMember(Player))
            {
                Scoring = new GameEventScoring[]
                {
                    new(attacker)
                    {
                        Karma = Gamemode.Game.Instance.Karma.CalculatePenalty(Math.Min(Damage, Player.Health))
                    }
                };
            }
        }

        public override void WriteData(BinaryWriter binaryWriter)
        {
            base.WriteData(binaryWriter);

            binaryWriter.Write(Damage);
            binaryWriter.Write(AttackerIdent);
            binaryWriter.Write(AttackerName);
            binaryWriter.Write(AttackerPlayerId ?? -1);
        }

        public override void ReadData(BinaryReader binaryReader)
        {
            base.ReadData(binaryReader);

            Damage = binaryReader.ReadSingle();
            AttackerIdent = binaryReader.ReadInt32();
            AttackerName = binaryReader.ReadString();
            AttackerPlayerId = binaryReader.ReadInt64();

            if (AttackerPlayerId == -1)
            {
                AttackerPlayerId = null;
            }
        }
    }
}
