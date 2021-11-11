namespace TTTReborn.Settings
{
    public partial class ServerSettings
    {
        public Categories.Round Round { get; set; } = new();
        public Categories.Map Map { get; set; } = new();
        public Categories.AFK AFK { get; set; } = new();
        public Categories.Debug Debug { get; set; } = new();
    }

    namespace Categories
    {
        public partial class Round
        {
            [InputSetting]
            public int MinPlayers { get; set; } = 2; // The minimum players required to start.

            [InputSetting]
            public int PreRoundTime { get; set; } = 20; // The amount of time allowed for preparation.

            [InputSetting]
            public int RoundTime { get; set; } = 300; // The amount of time allowed for the main round.

            [InputSetting]
            public int PostRoundTime { get; set; } = 10; // The amount of time before the next round starts.

            [InputSetting]
            public int MapSelectionRoundTime { get; set; } = 15; // The amount of time to vote for the next map.

            [InputSetting]
            public int TotalRounds { get; set; } = 10; // The amount of rounds to play.

            [InputSetting]
            public int KillTimeReward { get; set; } = 30; // The amount of extra time given to traitors for killing an innocent.
        }

        public partial class Map
        {
            [InputSetting]
            public string DefaultMap { get; set; } = "facepunch.flatgrass"; // The map we choose by default.
        }

        public partial class AFK
        {
            [SwitchSetting]
            public bool ShouldKickPlayers { get; set; } = false; // If we should kick the players instead of forcing them as spectators.

            [InputSetting]
            public int SecondsTillKick { get; set; } = 180; // The amount of time before we determine a player is AFK.
        }

        public partial class Debug
        {
            [SwitchSetting]
            public bool PreventWin { get; set; } = false; // If the round should never end (for debugging purposes)
        }
    }
}
