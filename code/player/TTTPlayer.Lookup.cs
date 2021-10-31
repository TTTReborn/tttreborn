using Sandbox;

using TTTReborn.UI;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        private T IsLookingAtType<T>(float distance)
        {
            Trace trace;

            if (IsClient)
            {
                Sandbox.Camera camera = Camera as Sandbox.Camera;

                trace = Trace.Ray(camera.Position, camera.Position + camera.Rotation.Forward * distance);
            }
            else
            {
                trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * distance);
            }

            trace = trace.HitLayer(CollisionLayer.Debris).Ignore(this);

            if (IsSpectatingPlayer)
            {
                trace = trace.Ignore(CurrentPlayer);
            }

            TraceResult tr = trace.UseHitboxes().Run();

            if (tr.Hit && tr.Entity is T type)
            {
                return type;
            }

            return default;
        }

        // Similar to "IsLookingAtType" but with an extra check ensuring we are within the range
        // of the "HintDistance".
        private IEntityHint IsLookingAtHintableEntity(float maxHintDistance)
        {
            Trace trace;

            if (IsClient)
            {
                Sandbox.Camera camera = Camera as Sandbox.Camera;

                trace = Trace.Ray(camera.Position, camera.Position + camera.Rotation.Forward * maxHintDistance);
            }
            else
            {
                trace = Trace.Ray(EyePos, EyePos + EyeRot.Forward * maxHintDistance);
            }

            trace = trace.HitLayer(CollisionLayer.Debris).Ignore(this);

            if (IsSpectatingPlayer)
            {
                trace = trace.Ignore(CurrentPlayer);
            }

            TraceResult tr = trace.UseHitboxes().Run();

            if (tr.Hit && tr.Entity is IEntityHint hint && tr.StartPos.Distance(tr.EndPos) <= hint.HintDistance)
            {
                return hint;
            }

            return null;
        }
    }
}
