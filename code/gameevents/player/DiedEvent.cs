namespace TTTReborn.Events.Player
{
    [GameEvent("player_died")]
    public partial class DiedEvent : PlayerGameEvent
    {
        /// <summary>
        /// Occurs when a player dies.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player who died.</para>
        /// </summary>
        public DiedEvent(TTTReborn.Player player) : base(player) { }

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
    }
}
