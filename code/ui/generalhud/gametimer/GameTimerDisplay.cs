using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Gamemode;

namespace TTTReborn.UI
{
    public class GameTimerDisplay : Panel
    {
        private readonly TranslationLabel _timerLabel;

        public GameTimerDisplay() : base()
        {
            StyleSheet.Load("/ui/generalhud/gametimer/GameTimerDisplay.scss");

            AddClass("center-horizontal");

            _timerLabel = Add.TranslationLabel();
            _timerLabel.AddClass("timer");
            _timerLabel.AddClass("center");
            _timerLabel.AddClass("text-shadow");

            AddChild(_timerLabel);
        }

        public override void Tick()
        {
            base.Tick();

            if (Game.Instance.Round == null)
            {
                return;
            }

            if (Game.Instance.Round is Rounds.WaitingRound)
            {
                _timerLabel.SetTranslation($"ROUND_STATE_{Game.Instance.Round.RoundName.ToUpper().Replace(' ', '_')}");
            }
            else
            {
                _timerLabel.Text = Game.Instance.Round.TimeLeftFormatted;
            }
        }
    }
}
