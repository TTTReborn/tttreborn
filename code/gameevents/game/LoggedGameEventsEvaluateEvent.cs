using System.Collections.Generic;

namespace TTTReborn.Events.Game
{
    [GameEvent("game_loggedgameeventevaluate"), Hammer.Skip]
    public partial class LoggedGameEventsEvaluateEvent : GameEvent
    {
        public List<ILoggedGameEvent> GameEvents { get; set; }

        /// <summary>
        /// Used to hook into the event.
        /// </summary>
        public LoggedGameEventsEvaluateEvent(List<ILoggedGameEvent> gameEvents) : base()
        {
            GameEvents = gameEvents ?? new();
        }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public LoggedGameEventsEvaluateEvent() : base() { }

        public override void Run() => Sandbox.Event.Run(Name, GameEvents);
    }
}
