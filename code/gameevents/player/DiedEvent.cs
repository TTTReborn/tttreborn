using Sandbox;

using TTTReborn.Globalization;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_died"), Hammer.Skip]
    public partial class DiedEvent : PlayerGameEvent, ILoggedGameEvent
    {
        public string LastAttackerName { get; set; }

        public bool IsAttackerPlayer { get; set; }

        public TranslationData GetDescriptionTranslationData()
        {
            if (Player != null && LastAttackerName != null)
            {
                if (LastAttackerName != PlayerName)
                {
                    return new(GetTranslationKey(IsAttackerPlayer ? "BYPLAYER" : "BYOBJECT"), PlayerName ?? "???", LastAttackerName ?? "???");
                }
                else
                {
                    return new(GetTranslationKey("BYSUICIDE"), PlayerName ?? "???");
                }
            }

            return new(GetTranslationKey("NOREASON"), PlayerName ?? "???");
        }

        /// <summary>
        /// Occurs when a player dies.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player who died.</para>
        /// </summary>
        public DiedEvent(TTTReborn.Player player) : base(player)
        {
            if (player != null && player.LastAttacker != null)
            {
                IsAttackerPlayer = player.LastAttacker is TTTReborn.Player;

                if (IsAttackerPlayer)
                {
                    LastAttackerName = player.LastAttacker.Client?.Name ?? "???";
                }
                else
                {
                    LastAttackerName = player.LastAttacker.Name;
                }
            }
        }

        protected override void OnRegister()
        {
            if (Player == null || !Player.IsValid || Player.LastAttacker is not TTTReborn.Player attacker)
            {
                return;
            }

            GameEventScoring attackerScoring = new(attacker);

            if (attacker.Role.Name.Equals(Player.Role.Name))
            {
                attackerScoring.Score = -2;
                attackerScoring.Karma = -100;
            }
            else if (attacker == Player)
            {
                attackerScoring.Score = -1;
            }
            else
            {
                attackerScoring.Score = 1;
                attackerScoring.Karma = 50;
            }

            Scoring = new GameEventScoring[]
            {
                new(Player)
                {
                    Score = -1
                },
                attackerScoring
            };
        }

        public bool Contains(Client client) => Name == client.Name || LastAttackerName == client.Name;
    }
}
