using System;
using System.Threading.Tasks;

using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class CountdownPerk : Perk
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
            while (Info.Owner != null)
            {
                try
                {
                    LastCountdown = 0f;

                    await GameTask.DelaySeconds(Countdown);

                    if (Info.Owner != null)
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
