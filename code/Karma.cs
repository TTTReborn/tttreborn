using System;
using Sandbox;
using System.Collections.Generic;

namespace TTTGamemode
{
    public partial class KarmaSystem
    {
        [ServerVar("ttt_karma_default", Help = "The default amount of karma given to a player.")]
        public static int TTTKarmaDefault { get; set; } = 1000;

        [ServerVar("ttt_karma_max", Help = "The maximum amount of karma a player can achieve.")]
        public static int TTTKarmaMax { get; set; } = 1250;

        [ServerVar("ttt_karma_min", Help = "The minimum amount of karma a player can have.")]
        public static int TTTKarmaMin { get; set; } = 500;

        [ServerVar("ttt_karma_ban", Help = "Should the player be banned once they reach minimum karma?")]
        public static bool TTTKarmaBan { get; set; } = true;

        [ServerVar("ttt_karma_gain", Help = "The amount of passive karma gain every round.")]
        public static int TTTKarmaGain { get; set; } = 50;

        [ServerVar("ttt_karma_loss", Help = "The amount of karma loss per damage dealt.")]
        public static float TTTKarmaLoss { get; set; } = 1f;

        [ServerVar("ttt_karma_penalty_max", Help = "The maximum amount of karma loss per player.")]
        public static int TTTKarmaPenaltyMax { get; set; } = 100;

        // TODO: Network dictionaries are not supported yet.
        // [Net] public Dictionary<string, int> KarmaRecords => new();
        // [Net] public Dictionary<(string, string), int> DamageRecords => new();
        
        [Net] 
        public bool IsTracking { get; set; }

        public void RegisterPlayer(TTTPlayer tttPlayer)
        {

        }

        public void RegisterPlayerDamage(TTTPlayer attacker, TTTPlayer victim, float damage)
        {
            if (!IsTracking)
                return;

            int updatedDamage = 0;
        }

        public void UpdatePlayerKarma(TTTPlayer tttPlayer, int delta)
        {
            UpdateSteamIdKarma(tttPlayer.GetClientOwner().SteamId, delta);
        }

        public void UpdateSteamIdKarma(ulong steamId, int delta)
        {
            int updatedKarma = 0;

            updatedKarma += delta;

            // Math.Clamp(updatedKarma, TTTKarmaMin, TTTKarmaMax)
            updatedKarma = updatedKarma > TTTKarmaMax ? TTTKarmaMax : updatedKarma;
            updatedKarma = updatedKarma < TTTKarmaMin ? TTTKarmaMin : updatedKarma;
        }

        public void ResolveKarma()
        {
            if (IsTracking)
            {

            }
        }
    }
}
