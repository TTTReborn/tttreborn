namespace TTTReborn.Events
{
    public static partial class TTTEvent
    {
        public static class Game
        {
            /// <summary>
            /// Should be used to precache models and stuff.
            /// <para>No data is passed to this event.</para>
            /// </summary>
            public const string Precache = "tttreborn.game.precache";

            /// <summary>
            /// Called everytime the round changes.
            /// <para>Event is passed the <strong><see cref="TTTReborn.Rounds.BaseRound"/></strong> instance of the old round.</para>
            /// <para>Event is passed the <strong><see cref="TTTReborn.Rounds.BaseRound"/></strong> instance of the new round.</para>
            /// </summary>
            public const string RoundChange = "tttreborn.game.roundchange";
        }
    }
}
