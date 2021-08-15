using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    /// <summary>
    /// C4 equipment definition, for the physical entity, see items/equipments/entities/C4Entity.cs
    /// </summary>
    [Library("ttt_c4"), Hammer.Skip]
    public partial class C4Equipment : TTTEquipment, IBuyableItem
    {
        public override string ViewModelPath => "";
        public override SlotType SlotType => SlotType.OffensiveEquipment;

        private const int PlaceDistance = 200;

        public int Price => 0;

        public C4Equipment() : base()
        {

        }

        public override void Spawn()
        {
            base.Spawn();

            RenderAlpha = 0f;
        }

        public override void Simulate(Client client)
        {
            if (!IsServer)
            {
                return;
            }

            if (Owner is not TTTPlayer owner)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1)) //Drops C4
                {
                    (owner.Inventory as Inventory).DropEntity(this, typeof(C4Entity));
                }
                else if (Input.Pressed(InputButton.Attack2)) //Plants C4
                {
                    TraceResult placementTrace = Trace.Ray(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * PlaceDistance)
                        .Ignore(owner)
                        .UseHitboxes()
                        .Run();
                    if (!placementTrace.Hit)
                    {
                        return;
                    }

                    C4Entity bomb = new C4Entity();
                    if (placementTrace.Entity.IsWorld)
                    {
                        bomb.PhysicsEnabled = false;
                    }
                    if (placementTrace.Entity is Prop prop)
                    {
                        bomb.CollisionGroup = CollisionGroup.Debris;
                        bomb.Parent = prop;
                    }

                    bomb.Position = placementTrace.EndPos;
                    bomb.WorldAng = placementTrace.Normal.EulerAngles;
                    bomb.Rotation = bomb.Rotation.RotateAroundAxis(Vector3.Right, -90);
                    bomb.Rotation = bomb.Rotation.RotateAroundAxis(Vector3.Up, 90);

                    if (placementTrace.Entity is TTTPlayer player)
                    {
                        bomb.EnableSolidCollisions = false;
                        bomb.AttachedBone = placementTrace.Bone;
                        bomb.SetParent(player, placementTrace.Bone);
                    }

                    (owner.Inventory as Inventory).Remove(this);
                }
            }
        }

        public override bool CanDrop() => false;
    }
}
