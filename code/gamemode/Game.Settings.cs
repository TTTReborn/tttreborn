namespace TTTReborn.Settings
{
    public partial class ServerSettings
    {
        public Topics.Round Round { get; set; } = new Topics.Round();
    }


    namespace Topics
    {
        public partial class Round
        {
            public int MinPlayers { get; set; } = 2; // The minimum players required to start.

            public int PreRoundTime { get; set; } = 20; // The amount of time allowed for preparation.

            public int RoundTime { get; set; } = 300; // The amount of time allowed for the main round.

            public int PostRoundTime { get; set; } = 10; // The amount of time before the next round starts.

            public int KillTimeReward { get; set; } = 30; // The amount of extra time given to traitors for killing an innocent.
        }
    }
}
