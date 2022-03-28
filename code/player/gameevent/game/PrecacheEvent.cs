namespace TTTReborn.Events
{
    public partial class Game
    {

        /// <summary>
        /// Should be used to precache models and stuff.
        /// </summary>
        [GameEvent("game_precache")]
        public partial class PrecacheEvent : GameEvent
        {

        }
    }
}
