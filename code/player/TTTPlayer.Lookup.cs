using Sandbox;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        public TTTPlayer Player { get; set; }

        private const float MAX_DRAW_DISTANCE = 500;

        private void TickAttemptLookup()
        {
            using (Prediction.Off())
            {
                To client = To.Single(this);
                TTTPlayer player = IsLookingAtPlayer();

                if (player.IsValid())
                {
                    // Send the request to the player looking at the player corpse.
                    // https://wiki.facepunch.com/sbox/RPCs#targetingplayers

                    ClientSetNameplateHealth(client, player.Health);
                }
            }
        }

        private TTTPlayer IsLookingAtPlayer()
        {
            TraceResult trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * MAX_DRAW_DISTANCE)
                .Ignore(ActiveChild)
                .Ignore(this)
                .UseHitboxes()
                .Run();

            if (trace.Hit && trace.Entity is TTTPlayer target)
            {
                return target;
            }

            return null;
        }
    }
}
