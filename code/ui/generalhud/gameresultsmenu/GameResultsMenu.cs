using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Rounds;

namespace TTTReborn.UI
{
    public class GameResultsMenu : Panel
    {
        public static GameResultsMenu Instance;

        public GameResultsMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/gameresultsmenu/GameResultsMenu.scss");

            AddClass("text-shadow");
        }

        [Event(typeof(Events.Game.GameResultsEvent))]
        protected void OnGameResultsEvent(List<GameEvent> gameEvents)
        {
            foreach (GameEvent gameEvent in gameEvents)
            {
                Add.Label(gameEvent.Name);
            }

            SetClass("shown", true);
        }

        [Event(typeof(Events.Game.RoundChangeEvent))]
        protected void OnRoundChangeEvent(BaseRound oldRound, BaseRound newRound)
        {
            if (oldRound is PostRound)
            {
                SetClass("shown", false);
                DeleteChildren(true);
            }
        }
    }
}
