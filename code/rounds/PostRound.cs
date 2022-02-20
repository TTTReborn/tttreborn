using Sandbox;

using TTTReborn.Player;
using TTTReborn.Settings;

namespace TTTReborn.Rounds
{
    public class PostRound : BaseRound
    {
        public override string RoundName => "Post";
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

        public override void OnPlayerKilled(TTTPlayer player)
        {
            player.MakeSpectator();
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                using (Prediction.Off())
                {
                    foreach (TTTPlayer player in Utils.GetPlayers())
                    {
                        if (player.PlayerCorpse != null && player.PlayerCorpse.IsValid() && player.LifeState == LifeState.Dead && !player.PlayerCorpse.IsIdentified)
                        {
                            player.PlayerCorpse.IsIdentified = true;

                            RPCs.ClientConfirmPlayer(null, player.PlayerCorpse, player, player.Role.Name, player.Team.Name, player.PlayerCorpse.GetConfirmationData(), player.PlayerCorpse.KillerWeapon, player.PlayerCorpse.Perks);
                        }
                        else
                        {
                            player.SendClientRole(To.Everyone);
                        }
                    }
                }
            }
        }
    }
}
