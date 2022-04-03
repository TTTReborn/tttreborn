using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class EventPanel : Panel
    {
        private TranslationLabel TitleLabel { get; set; }
        private TranslationLabel DescriptionLabel { get; set; }

        private float CreatedAt { get; set; }

        public string FormattedCreatedAt
        {
            get => $"[{Utils.TimerString(CreatedAt)}]";
        }

        private Panel EventIcon { get; set; }
        private Panel ScorePanel { get; set; }

        public EventPanel(ILoggedGameEvent loggedGameEvent)
        {
            CreatedAt = loggedGameEvent.CreatedAt;

            EventIcon.Style.SetBackgroundImage(Sandbox.Texture.Load(Sandbox.FileSystem.Mounted, $"assets/events/{loggedGameEvent.Name}.png", false) ?? Sandbox.Texture.Load(Sandbox.FileSystem.Mounted, $"assets/events/ttt_gameevent_base.png", false));
            TitleLabel?.UpdateTranslation(loggedGameEvent.GetTitleTranslationData());
            DescriptionLabel?.UpdateTranslation(loggedGameEvent.GetDescriptionTranslationData());

            GameEvent gameEvent = loggedGameEvent as GameEvent;

            if (gameEvent.Scoring == null)
            {
                ScorePanel.Enabled(false);

                return;
            }

            if (gameEvent.Scoring.Length > 0)
            {
                Panel header = ScorePanel.Add.Panel("entry header");
                header.Add.TranslationLabel(new("GAMERESULTSMENU.EVENTS.SCORING.PLAYERNAME"), "player");
                header.Add.TranslationLabel(new("GAMERESULTSMENU.EVENTS.SCORING.SCORE"), "score");
                header.Add.TranslationLabel(new("GAMERESULTSMENU.EVENTS.SCORING.KARMA"), "karma");

                foreach (GameEventScoring scoring in gameEvent.Scoring)
                {
                    if (scoring.Player == null || !scoring.Player.IsValid || scoring.Player.Client == null)
                    {
                        continue;
                    }

                    Panel entry = ScorePanel.Add.Panel("entry");
                    entry.Add.Label(scoring.Player.Client.Name, "player");
                    entry.Add.Label(Utils.GetFormattedNumber(scoring.Score), "score");
                    entry.Add.Label(Utils.GetFormattedNumber(scoring.Karma), "karma");
                }
            }
            else
            {
                ScorePanel.Enabled(false);
            }
        }
    }
}
