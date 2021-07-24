using Sandbox;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        public TTTPlayer Player { get; set; }

        private const float MAX_DRAW_DISTANCE = 500;

        public T IsLookingAtType<T>(float distance)
        {
            Sandbox.Camera camera = Camera as Sandbox.Camera;

            Trace trace = Trace.Ray(camera.Pos, camera.Pos + camera.Rot.Forward * distance)
                .HitLayer(CollisionLayer.Debris)
                .Ignore(this);

            if (IsSpectatingPlayer)
            {
                trace.Ignore(CurrentPlayer);
            }

            TraceResult tr = trace.Run();

            if (tr.Hit && tr.Entity is T type)
            {
                return type;
            }

            return default(T);
        }

        private TTTPlayer IsLookingAtPlayer()
        {
            return IsLookingAtType<TTTPlayer>(MAX_DRAW_DISTANCE);
        }
    }
}
