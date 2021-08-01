using Sandbox;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        public T IsLookingAtType<T>(float distance)
        {
            Trace trace;

            if (IsClient)
            {
                Sandbox.Camera camera = Camera as Sandbox.Camera;

                trace = Trace.Ray(camera.Pos, camera.Pos + camera.Rot.Forward * distance);
            }
            else
            {
                trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * distance);
            }

            trace = trace.HitLayer(CollisionLayer.Debris).Ignore(this);

            if (IsSpectatingPlayer)
            {
                trace.Ignore(CurrentPlayer);
            }

            TraceResult tr = trace.UseHitboxes().Run();

            if (tr.Hit && tr.Entity is T type)
            {
                return type;
            }

            return default;
        }
    }
}
