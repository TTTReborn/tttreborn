namespace TTTReborn.Events.Game
{
    [GameEvent("game_mapimagechange"), HideInEditor]
    public partial class MapImagesChangeEvent : GameEvent
    {
        /// <summary>
        /// Updates when the map images are networked.
        /// </summary>
        public MapImagesChangeEvent() : base() { }
    }
}
