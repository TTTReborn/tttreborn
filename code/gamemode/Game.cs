using System;
using Sandbox;

using TTTReborn.UI;
using TTTReborn.Player;
using TTTReborn.Rounds;

namespace TTTReborn.Gamemode
{
    [Library("tttreborn", Title = "Trouble in Terry's Town")]
    partial class Game : Sandbox.Game
    {
        public static Game Instance { get; private set; }

        [Net]
        public BaseRound Round { get; private set; } = new Rounds.WaitingRound();

        public KarmaSystem Karma { get; private set; } = new KarmaSystem();

        public Game()
        {
            Instance = this;

            if (IsServer)
            {
                new Hud();
            }
        }

        public void ChangeRound(BaseRound round)
        {
            Assert.NotNull(round);

            Round.Finish();
            Round = round;
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

            TTTPlayer player = new TTTPlayer();
            client.Pawn = player;
            player.InitialRespawn();

            base.ClientJoined(client);
        }

        public override void ClientDisconnect(Client client, NetworkDisconnectionReason reason)
        {
            Log.Info(client.Name + " left, checking minimum player count...");

            Round.OnPlayerLeave(client.Pawn as TTTPlayer);

            base.ClientDisconnect(client, reason);
        }

        public override void PostLevelLoaded()
        {
            StartGameTimer();

            base.PostLevelLoaded();
        }

        private async void StartGameTimer()
        {
            ChangeRound(new WaitingRound());

            while (true)
            {
                try
                {
                    OnGameSecond();

                    await GameTask.DelaySeconds(1);
                }
                catch(Exception e)
                {
                    if (e.Message.Trim() != "A task was canceled.")
                    {
                        Log.Error($"{e.Message}: {e.StackTrace}");
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void OnGameSecond()
        {
            Round?.OnSecond();
        }
    }
}
