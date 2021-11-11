namespace TTTReborn.Events
{
    public static partial class TTTEvent
    {
        public static class MapSelectionHandler
        {
            /// <summary>
            /// Updates when the map images are networked.
            /// <para>No data is passed to this event.</para>
            /// </summary>
            public const string MapImagesChange = "tttreborn.game.mapimagechange";
        }
    }
}
