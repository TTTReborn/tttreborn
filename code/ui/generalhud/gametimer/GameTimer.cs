using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Gamemode;

namespace TTTReborn.UI
{
    public class GameTimer : TTTPanel
    {
        private readonly GameTimerContent _gameTimerContent;

        public GameTimer()
        {
            StyleSheet.Load("/ui/generalhud/gametimer/GameTimer.scss");

            _gameTimerContent = new(this);
        }

        private class GameTimerContent : TTTPanel
        {
            private readonly TranslationLabel _textLabel;
            private readonly Label _timeLabel;

            public GameTimerContent(Panel parent)
            {
                Parent = parent;

                _textLabel = Add.TranslationLabel("", "textlabel");
                _timeLabel = Add.Label("", "timelabel");
            }
            public override void Tick()
            {
                base.Tick();

                if (Game.Instance.Round == null)
                {
                    return;
                }

                bool isWaitingRound = Game.Instance.Round is Rounds.WaitingRound;

                _textLabel.SetTranslation($"ROUND_STATE_{Game.Instance.Round.RoundName.ToUpper().Replace(' ', '_')}");

                _timeLabel.Text = isWaitingRound ? "" : Game.Instance.Round.TimeLeftFormatted;
                _timeLabel.SetClass("hide", isWaitingRound);
            }
        }
    }
}
