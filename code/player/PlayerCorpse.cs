using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TTTReborn.Player;

namespace TTTReborn.Player
{
	public class PlayerCorpse : ModelEntity
	{
		public TTTPlayer Player { get; set; }
		public bool IsIdentified { get; set; }

		public PlayerCorpse()
		{
			MoveType = MoveType.Physics;
			UsePhysicsCollision = true;

			SetInteractsAs(CollisionLayer.Debris);
			SetInteractsWith(CollisionLayer.WORLD_GEOMETRY);
			SetInteractsExclude(CollisionLayer.Player | CollisionLayer.Debris);

			IsIdentified = false;
		}

		public void CopyFrom(TTTPlayer player)
		{
			SetModel(player.GetModelName());
			TakeDecalsFrom(player);

			this.CopyBonesFrom(player);
			this.SetRagdollVelocityFrom(player);

			foreach (Entity child in player.Children)
			{
				if (child is ModelEntity e)
				{
					string model = e.GetModelName();

					if (model != null && !model.Contains("clothes"))
					{
						continue;
					}

					ModelEntity clothing = new ModelEntity();
					clothing.SetModel(model);
					clothing.SetParent(this, true);
				}
			}
		}

		public void ApplyForceToBone(Vector3 force, int forceBone)
		{
			PhysicsGroup.AddVelocity(force);

			if (forceBone >= 0)
			{
				PhysicsBody body = GetBonePhysicsBody(forceBone);

				if (body != null)
				{
					body.ApplyForce(force * 1000);
				}
				else
				{
					PhysicsGroup.AddVelocity(force);
				}
			}
		}
	}
}
