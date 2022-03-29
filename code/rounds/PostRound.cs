using Sandbox;

using TTTReborn.Settings;

namespace TTTReborn.Rounds
{
    public class PostRound : BaseRound
    {
        public override string RoundName { get; set; } = "Post";
        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.PostRoundTime;
        }

        protected override void OnTimeUp()
        {
            base.OnTimeUp();

            // TODO: Allow users to close the menu themselves using mouse cursor.
            RPCs.ClientClosePostRoundMenu();

            bool shouldChangeMap = Gamemode.Game.Instance.MapSelection.TotalRoundsPlayed >= ServerSettings.Instance.Round.TotalRounds;
            Gamemode.Game.Instance.ChangeRound(shouldChangeMap ? new MapSelectionRound() : new PreRound());
        }

        public override void OnPlayerKilled(Player player)
        {
            player.MakeSpectator();

            base.OnPlayerKilled(player);
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                using (Prediction.Off())
                {
                    foreach (Player player in Utils.GetPlayers())
                    {
                        if (player.PlayerCorpse != null && player.PlayerCorpse.IsValid() && player.LifeState == LifeState.Dead && !player.PlayerCorpse.Data.IsIdentified)
                        {
                            player.PlayerCorpse.Data.IsIdentified = true;

                            RPCs.ClientConfirmPlayer(null, player.PlayerCorpse, player.PlayerCorpse.GetSerializedData());
                        }
                        else
                        {
                            player.SendClientRole(To.Everyone);
                        }
                    }
                }
            }

            base.OnStart();
        }
    }
}
