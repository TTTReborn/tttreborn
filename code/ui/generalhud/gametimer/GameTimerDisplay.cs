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

            if (TTTGame.Instance.Round == null)
            {
                return;
            }

            RoundLabel.UpdateTranslation(new TranslationData($"ROUND.STATE.{TTTGame.Instance.Round.RoundName.ToUpper().Replace(' ', '_')}"));

            TimerPanel.SetClass("disabled", TTTGame.Instance.Round is Rounds.WaitingRound);
            TimerLabel.Text = TTTGame.Instance.Round.TimeLeftFormatted;
        }
    }
}
