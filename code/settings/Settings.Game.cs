namespace TTTReborn.Settings
{
    public partial class ServerSettings
    {
        public Categories.Round Round { get; set; } = new Categories.Round();
        public Categories.AFK AFK { get; set; } = new Categories.AFK();
        public Categories.Debug Debug { get; set; } = new Categories.Debug();
    }


    namespace Categories
    {
        public partial class Round
        {
            public int MinPlayers { get; set; } = 2; // The minimum players required to start.

            public int PreRoundTime { get; set; } = 20; // The amount of time allowed for preparation.

            public int RoundTime { get; set; } = 300; // The amount of time allowed for the main round.

            public int PostRoundTime { get; set; } = 10; // The amount of time before the next round starts.

            public int KillTimeReward { get; set; } = 30; // The amount of extra time given to traitors for killing an innocent.
        }

        public partial class AFK
        {
            public bool ShouldKickPlayers { get; set; } = false; // If we should kick the players instead of forcing them as spectators.
            public int SecondsTillKick { get; set; } = 180; // The amount of time before we determine a player is AFK.
        }

        public partial class Debug
        {
            public bool PreventWin { get; set; } = false; // If the round should never end (for debugging purposes)
        }
    }
}
