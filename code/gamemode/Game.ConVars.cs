namespace TTTReborn.Gamemode
{
    public partial class Game
    {
        [ServerVar("ttt_min_players", Help = "The minimum players required to start.")]
        public static int TTTMinPlayers { get; set; } = 2;

        [ServerVar("ttt_preround_timer", Help = "The amount of time allowed for preparation.")]
        public static int TTTPreRoundTime { get; set; } = 20;

        [ServerVar("ttt_round_timer", Help = "The amount of time allowed for the main round.")]
        public static int TTTRoundTime { get; set; } = 300;

        [ServerVar("ttt_postround_timer", Help = "The amount of time before the next round starts.")]
        public static int TTTPostRoundTime { get; set; } = 10;

        [ServerVar("ttt_kill_time_reward", Help = "The amount of extra time given to traitors for killing an innocent.")]
        public static int TTTKillTimeReward { get; set; } = 30;
    }

}
