using Sandbox;
using Sandbox.Joints;

using TTTReborn.Player;

// Credits to the PhysGun: https://github.com/Facepunch/sandbox/blob/master/code/tools/PhysGun.cs

namespace TTTReborn.Items
{
    [Equipment(SlotType = SlotType.UtilityEquipment)]
    [Library("ttt_hands")]
    partial class Hands : TTTEquipment
    {
        public override string ViewModelPath => "";

        public Entity GrabbedEntity { get; set; }

        public override void Spawn()
        {
            base.Spawn();

            RenderColor = Color.Transparent;
        }

        public override void Simulate(Client client)
        {
            if (!IsServer)
            {
                return;
            }

            if (Owner is not TTTPlayer player)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1) && GrabbedEntity == null)
                {
                    GrabEntity(player);
                }
                else if (Input.Released(InputButton.Attack2) && GrabbedEntity != null)
                {
                    DropEntity();
                }
            }
        }

        private void GrabEntity(TTTPlayer player)
        {
            if (GrabbedEntity.IsValid())
            {
                return;
            }

            Vector3 eyePos = player.EyePos;
            Vector3 eyeDir = player.EyeRot.Forward;

            TraceResult tr = Trace.Ray(eyePos, eyePos + eyeDir * 5000)
                .UseHitboxes()
                .Ignore(player)
                .HitLayer(CollisionLayer.Debris)
                .EntitiesOnly()
                .Run();

            if (!tr.Hit || !tr.Entity.IsValid() || tr.Entity is not ModelEntity)
            {
                return;
            }

            tr.Entity.SetParent(player, "middle_of_both_hands");
            GrabbedEntity = tr.Entity;
            GrabbedEntity.EnableHideInFirstPerson = false;
        }

        private void DropEntity()
        {
            if (!GrabbedEntity.IsValid())
            {
                return;
            }

            GrabbedEntity.SetParent(null);
            GrabbedEntity = null;
        }

        public override void ActiveEnd(Entity ent, bool dropped)
        {
            DropEntity();

            base.ActiveEnd(ent, dropped);
        }

        protected override void OnDestroy()
        {
            DropEntity();

            base.OnDestroy();
        }

        public override bool CanDrop() => false;
    }
}


