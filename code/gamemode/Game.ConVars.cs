namespace TTTReborn.Settings
{
    public partial class ServerSettings
    {
        public int TTTMinPlayers { get; set; } = 2; // The minimum players required to start.

        public int TTTPreRoundTime { get; set; } = 20; // The amount of time allowed for preparation.

        public int TTTRoundTime { get; set; } = 300; // The amount of time allowed for the main round.

        public int TTTPostRoundTime { get; set; } = 10; // The amount of time before the next round starts.

        public int TTTKillTimeReward { get; set; } = 30; // The amount of extra time given to traitors for killing an innocent.
    }
}
