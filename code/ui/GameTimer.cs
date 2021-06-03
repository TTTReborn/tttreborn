using Sandbox.UI;
using Sandbox.UI.Construct;

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
            // Round handling
        }
    }

}
