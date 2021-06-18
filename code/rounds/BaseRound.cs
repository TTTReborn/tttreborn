using Sandbox;
using System;
using System.Collections.Generic;

using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public abstract partial class BaseRound : NetworkComponent
    {
        public virtual int RoundDuration => 0;
        public virtual string RoundName => "";
        public virtual bool CanPlayerSuicide => true;

        public List<TTTPlayer> Players = new();

        public float RoundEndTime { get; set; }

        public float TimeLeft
        {
            get
            {
                return RoundEndTime - Sandbox.Time.Now;
            }
        }

        [Net] public string TimeLeftFormatted { get; set; }

        public void Start()
        {
            if (Host.IsServer && RoundDuration > 0)
            {
                RoundEndTime = Sandbox.Time.Now + RoundDuration;
                TimeLeftFormatted = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
            }

            OnStart();
        }

        public void Finish()
        {
            if (Host.IsServer)
            {
                RoundEndTime = 0f;
                Players.Clear();
            }

            OnFinish();
        }

        public void AddPlayer(TTTPlayer player)
        {
            Host.AssertServer();

            if (!Players.Contains(player))
            {
                Players.Add(player);
            }
        }

        public virtual void OnPlayerSpawn(TTTPlayer player) { }

        public virtual void OnPlayerKilled(TTTPlayer player) { }

        public virtual void OnPlayerLeave(TTTPlayer player)
        {
            Players.Remove(player);
        }

        public virtual void OnTick() { }

        public virtual void OnSecond()
        {
            if (Host.IsServer)
            {
                if (RoundEndTime > 0 && Sandbox.Time.Now >= RoundEndTime)
                {
                    RoundEndTime = 0f;

                    OnTimeUp();
                }
                else
                {
                    TimeLeftFormatted = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
                }
            }
        }

        protected virtual void OnStart() { }

        protected virtual void OnFinish() { }

        protected virtual void OnTimeUp() { }
    }
}
