using System;
using Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTTGamemode
{
    [ClassLibrary("ttt-reborn", Title = "Trouble in Terry's Town")]
    partial class Game : Sandbox.Game
    {
        public enum Round { Waiting, PreRound, InProgress, PostRound }

        public static Game Instance { get => Current as Game; }

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

        [Net] public Round CurrentRound => Game.Round.Waiting;
        [Net] public int TimeRemaining => 0;

        public KarmaSystem Karma => new KarmaSystem();

        #region TTT Methods
        private void ChangeRound(Round round)
        {
            switch (round)
            {
                case Game.Round.Waiting:
                    TimeRemaining = 0;
                    Karma.IsTracking = false;

                    break;

                case Game.Round.PreRound:
                    TimeRemaining = TTTPreRoundTime;

                    break;

                case Game.Round.InProgress:
                    TimeRemaining = TTTRoundTime;

                    #region Select Roles
                    int detectiveCount = (int) ((float) Sandbox.Player.All.Count * 0.125f);
                    int traitorCount = (int) Math.Max((float) Sandbox.Player.All.Count * 0.25f, 1f);

                    List<Player> _players = Sandbox.Player.ConvertAll(p => (Player) p);
                    Random random = new Random();

                    // SELECT DETECTIVES
                    for (int i = 0; i < detectiveCount; i++)
                    {
                        int randomID = random.Next(_players.Count);
                        _players[randomID].PlayerRole = Player.Role.Detective;

                        _players.RemoveAt(randomID);
                    }

                    // SELECT TRAITORS
                    for (int i = 0; i < traitorCount; i++)
                    {
                        int randomID = random.Next(_players.Count);
                        _players[randomID].PlayerRole = Player.Role.Traitor;

                        _players.RemoveAt(randomID);
                    }

                    // SET REMAINING PLAYERS TO INNOCENT
                    for (int i = 0; i < _players.Count; i++)
                    {
                        _players[i].PlayerRole = Player.Role.Innocent;
                    }
                    #endregion

                    Karma.IsTracking = true;

                    break;

                case Game.Round.PostRound:
                    TimeRemaining = TTTPostRoundTime;

                    Karma.ResolveKarma();
                    Karma.IsTracking = false;

                    break;
            }

            CurrentRound = round;
        }

        private void CheckMinimumPlayers()
        {
            if (Sandbox.Player.All.Count >= TTTMinPlayers)
            {
                if (CurrentRound == Round.Waiting)
                {
                    ChangeRound(Round.PreRound);
                }
            }
            else if (CurrentRound != Round.Waiting)
            {
                ChangeRound(Round.Waiting);
            }
        }

        private void CheckRoundState()
        {
            if (CurrentRound != Round.InProgress)
                return;

            bool traitorsDead = true;
            bool innocentsDead = true;

            Player player;

            // Check for alive players
            for (int i = 0; i < Sandbox.Player.All.Count; i++)
            {
                player = Sandbox.Player.All[i] as Player;

                if (player.LifeState == LifeState.Alive)
                    continue;

                if (player.Role == Player.Role.Traitor)
                {
                    traitorsDead = false;
                }
                else
                {
                    innocentsDead = false;
                }
            }

            // End this round if there is just one team alive
            if (innocentsDead || traitorsDead)
            {
                ChangeRound(Round.PostRound);
            }
        }

        private void UpdateRoundTimer()
        {
            if (CurrentRound == Round.Waiting)
                return;

            if (TimeRemaining == 0)
            {
                switch (CurrentRound)
                {
                    case Round.PreRound:
                        ChangeRound(Round.InProgress);

                        break;

                    case Round.InProgress:
                        ChangeRound(Round.PostRound);

                        break;

                    case Round.PostRound:
                        ChangeRound(Round.PreRound);

                        break;
                }
            }
            else
            {
                TimeRemaining--;
            }
        }
        #endregion

        #region Game Timer
        private async Task StartGameTimer()
        {
            while (true)
            {
                UpdateGameTimer();
                await Task.DelaySeconds(1);
            }
        }

        private void UpdateGameTimer()
        {
            CheckMinimumPlayers();
            CheckRoundState();
            UpdateRoundTimer();
        }
        #endregion

        #region Gamemode Overrides
        public override void DoPlayerNoclip(Sandbox.Player player)
        {
            // Do nothing. The player can't noclip in this mode.
        }

        public override void DoPlayerSuicide(Sandbox.Player player)
        {
            base.DoPlayerSuicide(player);
        }

        public override void PostLevelLoaded()
        {
            _ = StartGameTimer();

            base.PostLevelLoaded();
        }

        public override void PlayerKilled(Sandbox.Player player)
        {
            CheckRoundState();

            base.PlayerKilled(player);
        }

        public override void PlayerJoined(Player player)
        {
            Karma.RegisterPlayer(player);

            if (Karma.IsBanned(player))
            {
                KickPlayer(player);

                return;
            }

            base.PlayerJoined();
        }

        public override void PlayerDisconnected(Sandbox.Player player, NetworkDisconnectionReason reason)
        {
            Log.Info(player.Name + " left, checking minimum player count...");

            CheckRoundState();

            base.PlayerDisconnected(player, reason);
        }

        public override Player CreatePlayer() => new();
        #endregion
    }
}
