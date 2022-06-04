using System.Collections.Generic;

namespace TTTReborn.Events.Game
{
    [GameEvent("game_loggedgameeventevaluate"), HideInEditor]
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

        public override void Run() => Sandbox.Event.Run(Name, GameEvents);
    }
}
