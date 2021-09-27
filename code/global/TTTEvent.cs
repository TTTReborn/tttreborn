using Sandbox;

namespace TTTReborn.Globals
{
    public static class TTTEvent
    {
        public static class Player
        {
            public const string Died = "tttreborn.player.died";

            public class DiedAttribute : EventAttribute
            {
                public DiedAttribute() : base(Died) { }
            }
        }
    }
}
