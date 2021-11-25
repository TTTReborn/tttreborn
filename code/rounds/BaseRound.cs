// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public abstract partial class BaseRound : BaseNetworkable
    {
        public virtual int RoundDuration => 0;
        public virtual string RoundName => "";

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
            }

            OnFinish();
        }

        public virtual void OnPlayerSpawn(TTTPlayer player)
        {

        }

        public virtual void OnPlayerKilled(TTTPlayer player)
        {

        }

        public virtual void OnPlayerJoin(TTTPlayer player)
        {

        }


        public virtual void OnPlayerLeave(TTTPlayer player)
        {

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
