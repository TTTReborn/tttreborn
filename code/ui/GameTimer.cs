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
                TimeLabel = Add.Label("00:00", "timelabel");
            }
            public override void Tick()
            {
                // TODO: Handle if Instance is null and if Round is null.
                if (Game.Instance.Round is Rounds.WaitingRound)
                {
                    TextLabel.Text = $"{Game.Instance.Round.RoundName}...";
                    TimeLabel.Text = "";
                    TimeLabel.AddClass("waiting");
                } else {
                    TimeLabel.RemoveClass("waiting");
                }


                // AddClass("playing");
                TextLabel.Text = $"{Game.Instance.Round.RoundName}:";
                TimeLabel.Text = $"{Game.Instance.Round.TimeLeftFormatted}";
            }
        }
    }


}
