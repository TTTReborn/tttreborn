using System;

using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.Map;
using TTTReborn.Rounds;
using TTTReborn.Settings;
using TTTReborn.VisualProgramming;

namespace TTTReborn.Gamemode
{
    public partial class Game : Sandbox.Game
    {
        public static Game Instance { get; private set; }

        [Net, Change]
        public BaseRound Round { get; private set; } = new Rounds.WaitingRound();

        [Net]
        public MapSelectionHandler MapSelection { get; set; } = new();

        public KarmaSystem Karma { get; private set; } = new();

        public MapHandler MapHandler { get; private set; }

        // [ConVar.Replicated("ttt_debug")]
        public bool Debug { get; set; } = true;

        public Game()
        {
            Instance = this;

            if (IsServer)
            {
                PrecacheFiles();
            }

            TTTLanguage.Load();
            SettingsManager.Load();
            _ = MapSelection.Load();

            if (IsServer)
            {
                ShopManager.Load();
            }

            NodeStack.Load();
        }

        public override void Shutdown()
        {
            SettingsManager.Unload();

            base.Shutdown();
        }

        /// <summary>
        /// Changes the round if minimum players is met. Otherwise, force changes to "WaitingRound"
        /// </summary>
        /// <param name="round"> The round to change to if minimum players is met.</param>
        public void ChangeRound(BaseRound round)
        {
            Assert.NotNull(round);

            ForceRoundChange(Utils.HasMinimumPlayers() ? round : new WaitingRound());
        }

        /// <summary>
        /// Force changes a round regardless of player count.
        /// </summary>
        /// <param name="round"> The round to change to.</param>
        public void ForceRoundChange(BaseRound round)
        {
            Host.AssertServer();

            Round.Finish();

            BaseRound oldRound = Round;
            Round = round;

            GameEvent.Register(new Events.Game.RoundChangeEvent(oldRound, round));

            Round.Start();
        }

        public override void DoPlayerNoclip(Client client)
        {
            // Do nothing. The player can't noclip in this mode.
        }

        public override void DoPlayerSuicide(Client client)
        {
            if (client.Pawn is Player player && player.LifeState == LifeState.Alive)
            {
                base.DoPlayerSuicide(client);
            }
        }

        public override void OnKilled(Entity entity)
        {
            base.OnKilled(entity);

            if (entity is Player player)
            {
                Round.OnPlayerKilled(player);
            }
        }

        public override void ClientJoined(Client client)
        {
            Karma.RegisterPlayer(client);

            if (Karma.IsBanned(client))
            {
                client.Kick();

                return;
            }

            NetworkableGameEvent.RegisterNetworked(new Events.Player.ConnectedEvent(client));

            Player player = new();
            client.Pawn = player;
            player.InitialSpawn();

            Round.OnPlayerJoin(player);

            base.ClientJoined(client);
        }

        public override void ClientDisconnect(Client client, NetworkDisconnectionReason reason)
        {
            Log.Info(client.Name + " left, checking minimum player count...");

            Round.OnPlayerLeave(client.Pawn as Player);

            NetworkableGameEvent.RegisterNetworked(new Events.Player.DisconnectedEvent(client.PlayerId, reason));

            base.ClientDisconnect(client, reason);
        }

        public override bool CanHearPlayerVoice(Client source, Client dest)
        {
            Host.AssertServer();

            if (source.Name.Equals(dest.Name) || source.Pawn is not Player sourcePlayer || dest.Pawn is not Player destPlayer)
            {
                return false;
            }

            if (Round is InProgressRound && sourcePlayer.LifeState == LifeState.Dead && destPlayer.LifeState == LifeState.Alive)
            {
                return false;
            }

            if (sourcePlayer.IsTeamVoiceChatEnabled && destPlayer.Team != sourcePlayer.Team)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Someone is speaking via voice chat. This might be someone in your game,
        /// or in your party, or in your lobby.
        /// </summary>
        public override void OnVoicePlayed(long playerId, float level)
        {
            Client client = null;

            foreach (Client loopClient in Client.All)
            {
                if (loopClient.PlayerId == playerId)
                {
                    client = loopClient;

                    break;
                }
            }

            if (client == null || !client.IsValid())
            {
                return;
            }

            if (client.Pawn is Player player)
            {
                player.IsSpeaking = true;
            }

            UI.VoiceChatDisplay.Instance?.OnVoicePlayed(client, level);
        }

        public override void PostLevelLoaded()
        {
            StartGameTimer();

            base.PostLevelLoaded();

            MapHandler = new();
        }

        private async void StartGameTimer()
        {
            ForceRoundChange(new WaitingRound());

            while (true)
            {
                try
                {
                    OnGameSecond();

                    await GameTask.DelaySeconds(1);
                }
                catch (Exception e)
                {
                    if (e.Message.Trim() == "A task was canceled.")
                    {
                        return;
                    }

                    throw;
                }
            }
        }

        private void OnGameSecond()
        {
            Round?.OnSecond();
        }

        public static void OnRoundChanged(BaseRound oldRound, BaseRound newRound)
        {
            GameEvent.Register(new Events.Game.RoundChangeEvent(oldRound, newRound));
        }
    }
}
