using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public abstract partial class BaseRound : BaseNetworkable
    {
        public virtual int RoundDuration => 0;
        public virtual string RoundName => "";

        public readonly List<TTTPlayer> Players = new();
        public readonly List<TTTPlayer> Spectators = new();

        public float RoundEndTime { get; set; }

        public float TimeLeft => RoundEndTime - Time.Now;

        [Net]
        public string TimeLeftFormatted { get; set; }

        public void Start()
        {
            if (Host.IsServer && RoundDuration > 0)
            {
                RoundEndTime = Time.Now + RoundDuration;
                TimeLeftFormatted = Globals.Utils.TimerString(TimeLeft);
            }

            OnStart();
        }

        public void Finish()
        {
            if (Host.IsServer)
            {
                RoundEndTime = 0f;

                Players.Clear();
                Spectators.Clear();
            }

            OnFinish();
        }

        public void AddPlayer(TTTPlayer player)
        {
            Host.AssertServer();

            if (!player.IsForcedSpectator && !Players.Contains(player))
            {
                Players.Add(player);
            }
            else if (player.IsForcedSpectator && !Spectators.Contains(player))
            {
                Spectators.Add(player);
            }
        }

        public virtual void OnPlayerSpawn(TTTPlayer player)
        {

        }

        public virtual void OnPlayerKilled(TTTPlayer player)
        {

        }

        public virtual void OnPlayerLeave(TTTPlayer player)
        {
            Players.Remove(player);
            Spectators.Remove(player);
        }

        public virtual void OnTick()
        {

        }

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
                    TimeLeftFormatted = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
                }
            }
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnFinish()
        {

        }

        protected virtual void OnTimeUp()
        {

        }
    }
}
