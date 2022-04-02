using System.Collections.Generic;

namespace TTTReborn.Events.Game
{
    [GameEvent("game_loggedgameeventevaluate"), Hammer.Skip]
    public partial class LoggedGameEventEvaluateEvent : GameEvent
    {
        public List<ILoggedGameEvent> GameEvents { get; set; }

        /// <summary>
        /// Used to hook into the event.
        /// </summary>
        public LoggedGameEventEvaluateEvent(List<ILoggedGameEvent> gameEvents) : base()
        {
            GameEvents = gameEvents ?? new();
        }

        public override void Run() => Sandbox.Event.Run(Name, GameEvents);
    }
}
