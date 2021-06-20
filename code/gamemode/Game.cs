using Sandbox;
using System.Threading.Tasks;

using TTTReborn.UI;
using TTTReborn.Player;
using TTTReborn.Rounds;

namespace TTTReborn.Gamemode
{
    [Library("tttreborn", Title = "Trouble in Terry's Town")]
    partial class Game : Sandbox.Game
    {
        public static Game Instance { get => Current as Game; }

        [Net]
        public BaseRound Round { get; private set; } = new Rounds.WaitingRound();

        public KarmaSystem Karma = new KarmaSystem();

        public Task GameTimer { get; private set; }

        private bool _isShuttingdown = false;

        public Game()
        {
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
            GameTimer = StartGameTimer();

            base.PostLevelLoaded();
        }

        private async Task StartGameTimer()
        {
            ChangeRound(new WaitingRound());

            while (!_isShuttingdown)
            {
                OnGameSecond();

                await Task.DelaySeconds(1);
            }
        }

        private void OnGameSecond()
        {
            Round?.OnSecond();
        }

        public override void Shutdown()
        {
            _isShuttingdown = true;
            GameTimer = null;
            Round = null;

            base.Shutdown();
        }
    }
}
