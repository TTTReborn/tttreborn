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

using Sandbox;

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
