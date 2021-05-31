using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTTGamemode
{
    [Library("tttreborn", Title = "Trouble in Terry's Town")]
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

        [Net] public Round CurrentRound { get; private set; }
        [Net] public int TimeRemaining { get; private set; }

        public KarmaSystem Karma = new KarmaSystem();
        
        public Game()
        {
	        if ( IsServer )
	        {
				new Hud();
	        }
        }

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
                    
                    int detectiveCount = (int) (All.Count * 0.125f);
                    int traitorCount = (int) Math.Max(All.Count * 0.25f, 1f);

                    List<Player> _players = Client.All.ToList().ConvertAll(p => p.Pawn as TTTGamemode.Player);
                    Random random = new Random();

                    // SELECT DETECTIVES
                    for (int i = 0; i < detectiveCount; i++)
                    {
                        int randomId = random.Next(_players.Count);
                        _players[randomId].Role = RoleType.Detective;

                        _players.RemoveAt(randomId);
                    }

                    // SELECT TRAITORS
                    for (int i = 0; i < traitorCount; i++)
                    {
                        int randomId = random.Next(_players.Count);
                        _players[randomId].Role = RoleType.Traitor;

                        _players.RemoveAt(randomId);
                    }

                    // SET REMAINING PLAYERS TO INNOCENT
                    for ( int i = 0; i < _players.Count; ++i )
                    {
	                    _players[i].Role = RoleType.Innocent;
                    }

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

            // Check for alive players
            for (int i = 0; i < Client.All.Count; i++)
            {
	            Player player = Client.All[i].Pawn as TTTGamemode.Player;

	            if (player.LifeState == LifeState.Alive)
                    continue;

                if (player.Role == RoleType.Traitor)
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
        
        public override void DoPlayerNoclip(Client client)
        {
            // Do nothing. The player can't noclip in this mode.
        }

        public override void DoPlayerSuicide(Client client)
        {
            base.DoPlayerSuicide(client);
        }

        public override void PostLevelLoaded()
        {
            _ = StartGameTimer();

            base.PostLevelLoaded();
        }

        public override void OnKilled(Entity entity)
        {
	        Client client = entity.GetClientOwner();
	        if ( client != null )
	        {
		        CheckRoundState();
	        }

		    base.OnKilled(entity);
        }

        public override void ClientJoined(Client client)
        {
	        base.ClientJoined(client);
	        
	        Player player = new TTTGamemode.Player();
	        Karma.RegisterPlayer(player);
	        client.Pawn = player;
	        
	        // TODO: Check karma, below threshold kick player.
	        
	        player.Respawn();
        }

        public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
        {
            Log.Info(client.Name + " left, checking minimum player count...");

            CheckRoundState();

            base.ClientDisconnect(client, reason);
        }
    }
}
