namespace TTTReborn.Events
{
    public static partial class TTTEvent
    {
        public static class Game
        {
            /// <summary>
            /// Should be used to precache models and stuff.
            /// </summary>
            public const string PRECACHE = "tttreborn.game.precache";

            /// <summary>
            /// Called everytime the round changes.
            /// <para>Event is passed the <strong><see cref="TTTReborn.Rounds.BaseRound"/></strong> instance of the old round.</para>
            /// <para>Event is passed the <strong><see cref="TTTReborn.Rounds.BaseRound"/></strong> instance of the new round.</para>
            /// </summary>
            public const string ROUND_CHANGE = "tttreborn.game.roundchange";

            /// <summary>
            /// Updates when the map images are networked.
            /// </summary>
            public const string MAP_IMAGES_CHANGE = "tttreborn.game.mapimagechange";
        }
    }
}
