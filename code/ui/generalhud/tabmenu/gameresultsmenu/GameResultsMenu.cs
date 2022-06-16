using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class GameResultsMenu : Panel
    {
        public static GameResultsMenu Instance { get; set; }

        private Panel HeaderPanel { get; set; }
        private Panel EventWrapper { get; set; }

        private TranslationLabel TitleLabel { get; set; }
        private TranslationButton PlayerButton { get; set; }
        private TranslationButton AllButton { get; set; }

        [Sandbox.SkipHotload]
        private List<ILoggedGameEvent> LoggedGameEvents { get; set; } = new();

        public GameResultsMenu()
        {
            Instance = this;

            TitleLabel.UpdateTranslation(new("GAMERESULTSMENU.TITLE"));
            PlayerButton.UpdateTranslation(new("GAMERESULTSMENU.PLAYERFILTER"));
            AllButton.UpdateTranslation(new("GAMERESULTSMENU.ALLFILTER"));

            Init();
        }

        private void Init()
        {
            bool isEmpty = LoggedGameEvents.Count == 0;

            EventWrapper.SetClass("empty", isEmpty);
            HeaderPanel.Enabled(!isEmpty);

            if (!isEmpty)
            {
                PlayerButton.AddEventListener("onclick", (e) => FilterEvents(false));
                AllButton.AddEventListener("onclick", (e) => FilterEvents(true));

                FilterEvents(true);
            }
            else
            {
                EventWrapper.Add.TranslationLabel(new("GAMERESULTSMENU.EMPTY"));
            }
        }

        private void FilterEvents(bool isAll)
        {
            EventWrapper.DeleteChildren(true);

            PlayerButton.SetClass("selected", !isAll);
            AllButton.SetClass("selected", isAll);

            if (isAll)
            {
                foreach (ILoggedGameEvent gameEvent in LoggedGameEvents)
                {
                    EventWrapper.AddChild(gameEvent.GetEventPanel());
                }
            }
            else
            {
                foreach (ILoggedGameEvent gameEvent in LoggedGameEvents)
                {
                    if (gameEvent.Contains(Sandbox.Local.Client))
                    {
                        EventWrapper.AddChild(gameEvent.GetEventPanel(Sandbox.Local.Client));
                    }
                }
            }
        }

        [Event("game_gameresult")]
        protected void OnGameResultsEvent(List<ILoggedGameEvent> gameEvents)
        {
            LoggedGameEvents = gameEvents;

            Init();
        }
    }
}
