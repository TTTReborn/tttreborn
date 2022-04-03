using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class GameResultsMenu : Panel
    {
        public static GameResultsMenu Instance { get; set; }

        private Panel EventWrapper { get; set; }

        public GameResultsMenu()
        {
            Instance = this;

            EventWrapper.Add.TranslationLabel(new("GAMERESULTSMENU.EMPTY"));
        }

        [Event(typeof(Events.Game.GameResultsEvent))]
        protected void OnGameResultsEvent(List<ILoggedGameEvent> gameEvents)
        {
            EventWrapper.DeleteChildren(true);
            EventWrapper.SetClass("empty", gameEvents.Count == 0);

            foreach (ILoggedGameEvent gameEvent in gameEvents)
            {
                EventWrapper.AddChild(gameEvent.GetEventPanel());
            }
        }
    }
}
