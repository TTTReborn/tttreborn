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
        }
    }
}
