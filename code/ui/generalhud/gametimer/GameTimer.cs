using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Gamemode;
using TTTReborn.Language;

namespace TTTReborn.UI
{
    public class GameTimer : TTTPanel
    {
        private GameTimerContent _gameTimerContent;

        public GameTimer()
        {
            StyleSheet.Load("/ui/generalhud/gametimer/GameTimer.scss");

            _gameTimerContent = new GameTimerContent(this);
        }

        private class GameTimerContent : TTTPanel
        {
            private readonly Label _textLabel;
            private readonly Label _timeLabel;
            private TLanguage lang;

            public GameTimerContent(Panel parent)
            {
                Parent = parent;

                lang = TTTLanguage.GetActiveLanguage();

                _textLabel = Add.Label("", "textlabel");
                _timeLabel = Add.Label("", "timelabel");
            }
            public override void Tick()
            {
                base.Tick();

                if (Game.Instance.Round == null)
                {
                    return;
                }

                lang = TTTLanguage.GetActiveLanguage();

                bool isWaitingRound = Game.Instance.Round is Rounds.WaitingRound;

                switch (Game.Instance.Round)
                {
                    case Rounds.WaitingRound:
                        _textLabel.Text = lang.GetTranslation("ROUND_STATE_WAITING");
                        break;

                    case Rounds.PreRound:
                        _textLabel.Text = lang.GetTranslation("ROUND_STATE_PREPARING");
                        break;

                    case Rounds.InProgressRound:
                        _textLabel.Text = lang.GetTranslation("ROUND_STATE_IN_PROGRESS");
                        break;

                    case Rounds.PostRound:
                        _textLabel.Text = lang.GetTranslation("ROUND_STATE_POST_ROUND");
                        break;
                }
                _timeLabel.Text = isWaitingRound ? "" : Game.Instance.Round.TimeLeftFormatted;

                _timeLabel.SetClass("hide", isWaitingRound);
            }
        }
    }
}
