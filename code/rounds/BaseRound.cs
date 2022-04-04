using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Rounds
{
    public abstract partial class BaseRound : BaseNetworkable
    {
        public List<GameEvent> GameEvents { get; set; } = new();

        public virtual int RoundDuration { get; set; } = 0;
        public virtual string RoundName { get; set; } = "";

        [Net]
        public float RoundEndTime { get; set; }

        [Net]
        public float TimeLeft => RoundEndTime - Time.Now;

        [Net]
        public float StartedAt { get; set; }

        [Net]
        public string TimeLeftFormatted { get; set; }

        public void Start()
        {
            if (Host.IsServer)
            {
                StartedAt = Time.Now;

                if (RoundDuration > 0)
                {
                    RoundEndTime = Time.Now + RoundDuration;
                    TimeLeftFormatted = Utils.TimerString(TimeLeft);
                }
            }

            OnStart();
        }

        public void Finish()
        {
            if (Host.IsServer)
            {
                RoundEndTime = 0f;
            }

            OnFinish();
        }

        public virtual void OnPlayerSpawn(Player player)
        {
            bool handsAdded = player.Inventory.TryAdd(new Hands(), deleteIfFails: true, makeActive: true);

            Log.Debug($"Attempted to add Hands to {player.Client.Name}. Result: '{handsAdded}'");
        }

        public virtual void OnPlayerKilled(Player player) { }

        public virtual void OnPlayerJoin(Player player) { }

        public virtual void OnPlayerLeave(Player player) { }

        public virtual void OnTick() { }

        public virtual void OnSecond()
        {
            if (Host.IsServer)
            {
                if (RoundEndTime > 0 && Time.Now >= RoundEndTime)
                {
                    RoundEndTime = 0f;

                    OnTimeUp();
                }
                else
                {
                    TimeLeftFormatted = Utils.TimerString(TimeLeft);
                }
            }
        }

        protected virtual void OnStart() { }

        protected virtual void OnFinish()
        {
            if (!Host.IsServer)
            {
                return;
            }

            foreach (GameEvent gameEvent in GameEvents)
            {
                foreach (GameEventScoring scoring in gameEvent.Scoring)
                {
                    scoring.Evaluate();
                }
            }
        }

        protected virtual void OnTimeUp() { }
    }
}
