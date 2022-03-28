namespace TTTReborn.Events
{
    public static partial class TTTEvent
    {
        public static class Game
        {
            /// <summary>
            /// Updates when the map images are networked.
            /// </summary>
            public const string MAP_IMAGES_CHANGE = "tttreborn.game.mapimagechange";
        }
    }
}
