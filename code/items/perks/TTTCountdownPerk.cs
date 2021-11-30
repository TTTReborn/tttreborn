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
using System.Threading.Tasks;

using Sandbox;

namespace TTTReborn.Items
{
    [Hammer.Skip]
    public abstract class TTTCountdownPerk : TTTPerk
    {
        public abstract float Countdown { get; }

        public Task CountdownTask { get; private set; }

        public TimeSince LastCountdown { get; private set; } = 0f;

        public override void OnEquip()
        {
            base.OnEquip();

            CountdownTask = StartCountdown();
        }

        private async Task StartCountdown()
        {
            while (Owner != null)
            {
                try
                {
                    LastCountdown = 0f;

                    await GameTask.DelaySeconds(Countdown);

                    if (Owner != null)
                    {
                        OnCountdown();
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Trim() == "A task was canceled.")
                    {
                        return;
                    }

                    Log.Error($"[TASK] {e.Message}: {e.StackTrace}");
                }
            }
        }

        public abstract void OnCountdown();
    }
}
