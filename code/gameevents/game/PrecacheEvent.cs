namespace TTTReborn.Events.Game
{
    [GameEvent("game_precache"), HideInEditor]
    public partial class PrecacheEvent : GameEvent
    {
        /// <summary>
        /// Should be used to precache models and stuff.
        /// </summary>
        public PrecacheEvent() : base() { }
    }
}
