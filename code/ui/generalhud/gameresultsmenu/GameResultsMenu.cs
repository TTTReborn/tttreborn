using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Rounds;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class GameResultsMenu : Panel
    {
        public static GameResultsMenu Instance;

        private Panel EventWrapper { get; set; }

        public GameResultsMenu()
        {
            Instance = this;

            AddClass("text-shadow");
            AddClass("gameresultsmenu");
        }

        [Event(typeof(Events.Game.GameResultsEvent))]
        protected void OnGameResultsEvent(List<ILoggedGameEvent> gameEvents)
        {
            EventWrapper.DeleteChildren(true);

            foreach (ILoggedGameEvent gameEvent in gameEvents)
            {
                EventWrapper.AddChild(gameEvent.GetEventPanel());
            }

            SetClass("shown", true);
        }

        [Event(typeof(Events.Game.RoundChangeEvent))]
        protected void OnRoundChangeEvent(BaseRound oldRound, BaseRound newRound)
        {
            if (oldRound is PostRound)
            {
                SetClass("shown", false);
            }
        }
    }
}
