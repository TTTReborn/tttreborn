using Sandbox.UI;
using Sandbox.UI.Construct;
using TTTReborn.Gamemode;

namespace TTTReborn.UI
{
    public class GameTimer : Panel
    {

        private GameTimerContent gameTimerContent;
        public GameTimer()
        {
            StyleSheet.Load("/ui/GameTimer.scss");

            gameTimerContent = new GameTimerContent(this);
        }

        private class GameTimerContent : Panel
        {
            public Label TextLabel { set; get; }
            public Label TimeLabel { set; get; }

            public GameTimerContent(Panel parent)
            {
                Parent = parent;

                TextLabel = Add.Label("asd", "textlabel");
                TimeLabel = Add.Label("", "timelabel");
            }
            public override void Tick()
            {
                // TODO: Handle if Instance is null and if Round is null.
                bool isWaitingRound = Game.Instance.Round is Rounds.WaitingRound;
                if (isWaitingRound)
                {
                    TextLabel.Text = $"{Game.Instance.Round.RoundName}...";
                }
                else
                {
                    TextLabel.Text = $"{Game.Instance.Round.RoundName}:";
                    TimeLabel.Text = $"{Game.Instance.Round.TimeLeftFormatted}";
                }
                TimeLabel.AddClass("hide", isWaitingRound);
                TimeLabel.AddClass("waiting", isWaitingRound);


                // AddClass("playing");
            }
        }
    }


}
