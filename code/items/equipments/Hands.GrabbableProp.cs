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

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public class GrabbableProp : IGrabbable
    {
        public const float THROW_FORCE = 500;
        public Entity GrabbedEntity { get; set; }
        public TTTPlayer _owner;

        public bool IsHolding
        {
            get => GrabbedEntity != null;
        }

        public GrabbableProp(TTTPlayer player, Entity ent)
        {
            _owner = player;

            GrabbedEntity = ent;
            GrabbedEntity.SetParent(player, Hands.MIDDLE_HANDS_ATTACHMENT, new Transform(Vector3.Zero, Rotation.FromRoll(-90)));
            GrabbedEntity.EnableHideInFirstPerson = false;
        }

        public void Drop()
        {
            if (GrabbedEntity?.IsValid ?? false)
            {
                GrabbedEntity.EnableHideInFirstPerson = true;
                GrabbedEntity.SetParent(null);
            }

            GrabbedEntity = null;
        }

        public void Update(TTTPlayer player)
        {
            // If the entity is destroyed drop it.
            if (!GrabbedEntity?.IsValid ?? true)
            {
                Drop();

                return;
            }

            // If the entity gets another owner (i.e weapon pickup) drop it.
            if (GrabbedEntity?.Owner != null)
            {
                // Since the weapon now has a new owner/parent, no need to set parent to null.
                GrabbedEntity.EnableHideInFirstPerson = true;
                GrabbedEntity = null;

                return;
            }
        }

        public void SecondaryAction()
        {
            _owner.SetAnimBool("b_attack", true);

            GrabbedEntity.SetParent(null);
            GrabbedEntity.EnableHideInFirstPerson = true;
            GrabbedEntity.Velocity += _owner.EyeRot.Forward * THROW_FORCE;

            _ = WaitForAnimationFinish();
        }

        private async Task WaitForAnimationFinish()
        {
            try
            {
                await GameTask.DelaySeconds(0.6f);
            }
            catch (Exception e)
            {
                if (e.Message.Trim() != "A task was canceled.")
                {
                    Log.Error($"{e.Message}: {e.StackTrace}");
                }
            }
            finally
            {
                GrabbedEntity = null;
            }
        }
    }
}
