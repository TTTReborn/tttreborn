namespace TTTReborn.Events.Game
{
    [GameEvent("game_mapimagechange")]
    public partial class MapImagesChangeEvent : ParameterlessGameEvent
    {
        /// <summary>
        /// Updates when the map images are networked.
        /// </summary>
        public MapImagesChangeEvent() : base() { }
    }
}
