using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Gamemode;

namespace TTTReborn.UI
{
    public class GameTimerDisplay : Panel
    {
        private readonly TranslationLabel _roundLabel;
        private readonly Label _timerLabel;

        public GameTimerDisplay() : base()
        {
            StyleSheet.Load("/ui/generalhud/gametimer/GameTimerDisplay.scss");

            Style.BackgroundColor = ColorScheme.Primary;
            AddClass("centered-horizontal");
            AddClass("rounded-bottom");
            AddClass("opacity-90");
            AddClass("text-shadow");

            _roundLabel = Add.TranslationLabel();
            _roundLabel.AddClass("round-label");

            _timerLabel = Add.Label();
            _timerLabel.AddClass("timer-label");
        }

        public override void Tick()
        {
            base.Tick();

            if (Game.Instance.Round == null)
            {
                return;
            }

            _roundLabel.SetTranslation($"ROUND_STATE_{Game.Instance.Round.RoundName.ToUpper().Replace(' ', '_')}");

            _timerLabel.SetClass("disabled", Game.Instance.Round is Rounds.WaitingRound);
            _timerLabel.Text = Game.Instance.Round.TimeLeftFormatted;
        }
    }
}
