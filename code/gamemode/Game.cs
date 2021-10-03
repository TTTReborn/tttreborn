using System;
using System.Linq;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globalization;
using TTTReborn.Globals;
using TTTReborn.Map;
using TTTReborn.Player;
using TTTReborn.Rounds;
using TTTReborn.Settings;
using TTTReborn.UI;

namespace TTTReborn.Gamemode
{
    [Library("tttreborn", Title = "Trouble in Terry's Town"), Hammer.Skip]
    partial class Game : Sandbox.Game
    {
        public static Game Instance { get; private set; }

        [Net]
        public BaseRound Round { get; private set; } = new Rounds.WaitingRound();

        public KarmaSystem Karma { get; private set; } = new();

        public TTTMapSettings MapSettings { get; private set; }

        public Game()
        {
            Instance = this;

            TTTLanguage.LoadLanguages();
            SettingsManager.Load();

            if (IsServer)
            {
                ShopManager.Load();

                new Hud();
            }
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
            Round.Finish();
            Round = round;
            Event.Run("tttreborn.round.changed", round);
            Round.Start();
        }

        public override void DoPlayerNoclip(Client client)
        {
            // Do nothing. The player can't noclip in this mode.
        }

        public override void DoPlayerSuicide(Client client)
        {
            base.DoPlayerSuicide(client);
        }

        public override void OnKilled(Entity entity)
        {
            if (entity is TTTPlayer player)
            {
                Round.OnPlayerKilled(player);
            }

            base.OnKilled(entity);
        }

        public override void ClientJoined(Client client)
        {
            /*
            // TODO: KarmaSystem is waiting on network dictionaries.
            Karma.RegisterPlayer(client);

            if (Karma.IsBanned(player))
            {
                KickPlayer(player);

                return;
            }
            */

            Event.Run(TTTEvent.Player.Connected, client);

            RPCs.ClientOnPlayerConnected(client);

            TTTPlayer player = new();
            client.Pawn = player;
            player.InitialSpawn();

            base.ClientJoined(client);
        }

        public override void ClientDisconnect(Client client, NetworkDisconnectionReason reason)
        {
            Log.Info(client.Name + " left, checking minimum player count...");

            Round.OnPlayerLeave(client.Pawn as TTTPlayer);

            Event.Run(TTTEvent.Player.Disconnected, client.SteamId, reason);

            RPCs.ClientOnPlayerDisconnect(client.SteamId, reason);

            base.ClientDisconnect(client, reason);
        }

        public override bool CanHearPlayerVoice(Client source, Client dest)
        {
            Host.AssertServer();

            if (source.Pawn is not TTTPlayer sourcePlayer || dest.Pawn is not TTTPlayer destPlayer)
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
        public override void OnVoicePlayed(ulong steamId, float level)
        {
            Client client = null;

            foreach (Client loopClient in Client.All)
            {
                if (loopClient.SteamId == steamId)
                {
                    client = loopClient;

                    break;
                }
            }

            if (client == null || !client.IsValid())
            {
                return;
            }

            if (client.Pawn is TTTPlayer player)
            {
                player.IsSpeaking = true;
            }

            UI.VoiceChatDisplay.Instance?.OnVoicePlayed(client, level);
        }

        public override void PostLevelLoaded()
        {
            StartGameTimer();

            MapSettings = (TTTMapSettings) All.FirstOrDefault(x => x.GetType().Equals(typeof(TTTMapSettings)));
            MapSettings?.FireSettingsSpawn();

            base.PostLevelLoaded();
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

                    Log.Error($"{e.Message}: {e.StackTrace}");
                }
            }
        }

        private void OnGameSecond()
        {
            Round?.OnSecond();
        }
    }
}
