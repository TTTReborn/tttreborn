using Sandbox;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        public TTTPlayer Player { get; set; }

        private const float MAX_DRAW_DISTANCE = 500;

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
