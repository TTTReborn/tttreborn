using Sandbox;

using TTTReborn.Entities;

namespace TTTReborn
{
    partial class Player
    {
        private T IsLookingAtType<T>(float distance)
        {
            Trace trace;

            if (IsClient)
            {
                trace = Trace.Ray(CameraMode.Position, CameraMode.Position + CameraMode.Rotation.Forward * distance);
            }
            else
            {
                trace = Trace.Ray(EyePosition, EyePosition + EyeRotation.Forward * distance);
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

        public Trace GetLookingTrace(float maxHintDistance)
        {
            Trace trace;

            if (IsClient)
            {
                trace = Trace.Ray(CameraMode.Position, CameraMode.Position + CameraMode.Rotation.Forward * maxHintDistance);
            }
            else
            {
                trace = Trace.Ray(EyePosition, EyePosition + EyeRotation.Forward * maxHintDistance);
            }

            trace = trace.HitLayer(CollisionLayer.Debris).Ignore(this);

            if (IsSpectatingPlayer)
            {
                trace = trace.Ignore(CurrentPlayer);
            }

            return trace;
        }

        // Similar to "IsLookingAtType" but with an extra check ensuring we are within the range
        // of the "HintDistance".
        private IEntityHint IsLookingAtHintableEntity(float maxHintDistance)
        {
            TraceResult tr = GetLookingTrace(maxHintDistance).UseHitboxes().Run();

            if (tr.Hit && tr.Entity is IEntityHint hint && tr.StartPosition.Distance(tr.EndPosition) <= hint.HintDistance)
            {
                return hint;
            }

            return null;
        }

        private static IUse GetUsableParent(Entity entity)
        {
            Entity parent = entity;

            while (parent != null)
            {
                if (parent is IUse use)
                {
                    return use;
                }

                parent = parent.Parent;
            }

            return null;
        }

        private IUse IsLookingAtUsableEntity(float maxHintDistance)
        {
            TraceResult tr = GetLookingTrace(maxHintDistance).UseHitboxes().Run();

            if (tr.Hit)
            {
                IUse use = GetUsableParent(tr.Entity);

                return use;
            }

            return null;
        }
    }
}
