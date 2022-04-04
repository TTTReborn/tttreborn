using Sandbox;

using TTTReborn.Rounds;

#pragma warning disable IDE0051

namespace TTTReborn.Map
{
    [Library("ttt_map_settings")]
    public partial class MapSettings : Entity
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

        [Event(typeof(Events.Game.RoundChangeEvent))]
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
