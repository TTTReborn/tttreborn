using Sandbox.UI;
using Sandbox.UI.Construct;
using TTTReborn.Gamemode;

namespace TTTReborn.UI
{
    public class GameTimer : Panel
    {
        public Label TimeLabel { set; get; }

        public GameTimer()
        {
            StyleSheet.Load("/ui/GameTimer.scss");

            TimeLabel = Add.Label("00:00", "timelabel");
        }

        public override void Tick()
        {
            // TODO: Handle if Instance is null and if Round is null.
            if (Game.Instance.Round is Rounds.WaitingRound)
            {
                TimeLabel.Text = $"{Game.Instance.Round.RoundName}...";

                return;
            }

            TimeLabel.Text = $"{Game.Instance.Round.RoundName}: {Game.Instance.Round.TimeLeftFormatted}";
        }
    }

}
