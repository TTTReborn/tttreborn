using Sandbox.UI;
using Sandbox.UI.Construct;
using TTTReborn.Gamemode;

namespace TTTReborn.UI
{
    public class GameTimer : Panel
    {
        private GameTimerContent _gameTimerContent;

        public GameTimer()
        {
            StyleSheet.Load("/ui/GameTimer.scss");

            _gameTimerContent = new GameTimerContent(this);
        }

        private class GameTimerContent : Panel
        {
            private readonly Label _textLabel;
            private readonly Label _timeLabel;

            public GameTimerContent(Panel parent)
            {
                Parent = parent;

                _textLabel = Add.Label("asd", "textlabel");
                _timeLabel = Add.Label("", "timelabel");
            }
            public override void Tick()
            {
                if (Game.Instance.Round == null)
                {
                    return;
                }

                bool isWaitingRound = Game.Instance.Round is Rounds.WaitingRound;

                _textLabel.Text = $"{Game.Instance.Round.RoundName}:";
                _timeLabel.Text = isWaitingRound ? "" : $"{Game.Instance.Round.TimeLeftFormatted}";

                _timeLabel.SetClass("hide", isWaitingRound);
            }
        }
    }
}
