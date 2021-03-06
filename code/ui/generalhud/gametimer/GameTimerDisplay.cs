using Sandbox.UI;

using TTTReborn.Gamemode;
using TTTReborn.Globalization;

#pragma warning disable IDE0051

namespace TTTReborn.UI
{
    [UseTemplate]
    public class GameTimerDisplay : Panel
    {
        private Panel TimerPanel { get; set; }
        private Label TimerLabel { get; set; }
        private Panel RoundPanel { get; set; }
        private TranslationLabel RoundLabel { get; set; }

        public override void Tick()
        {
            base.Tick();

            if (Game.Instance.Round == null)
            {
                return;
            }

            RoundLabel.UpdateTranslation(new TranslationData($"ROUND.STATE.{Game.Instance.Round.RoundName.ToUpper().Replace(' ', '_')}"));

            TimerPanel.SetClass("disabled", Game.Instance.Round is Rounds.WaitingRound);
            TimerLabel.Text = Game.Instance.Round.TimeLeftFormatted;
        }
    }
}
