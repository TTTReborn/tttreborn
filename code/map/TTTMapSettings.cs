using Sandbox;

using TTTReborn.Events;
using TTTReborn.Rounds;

namespace TTTReborn.Map
{
    [Library("ttt_map_settings")]
    public partial class TTTMapSettings : Entity
    {
        /// <summary>
        /// Fired after PostLevelLoaded runs and MapSettings entity is found.
        /// </summary>
        protected Output SettingsSpawned { get; set; }
        /// <summary>
        /// Fired once Preround begins.
        /// </summary>
        protected Output RoundPreparation { get; set; }
        /// <summary>
        /// Fired once round starts and roles are assigned.
        /// </summary>
        protected Output RoundStart { get; set; }
        /// <summary>
        /// Fired once a win condition is met.
        /// </summary>
        protected Output RoundEnd { get; set; }

        /// <summary>
        /// Does not run on entity awake/spawn, is called explicitly by the TTT gamemode to trigger.
        /// </summary>
        public void FireSettingsSpawn() => SettingsSpawned.Fire(this);

        [Event(TTTEvent.Game.ROUND_CHANGE)]
        private void FireRoundChange(BaseRound _, BaseRound newRound)
        {
            switch (newRound)
            {
                case PreRound:
                    RoundPreparation.Fire(this);

                    break;

                case InProgressRound:
                    RoundStart.Fire(this);

                    break;

                case PostRound:
                    RoundEnd.Fire(this);

                    break;
            }
        }
    }
}
