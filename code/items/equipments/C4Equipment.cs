using Sandbox;

namespace TTTReborn.Items
{
    /// <summary>
    /// C4 equipment definition, for the physical entity, see items/equipments/entities/C4Entity.cs
    /// </summary>
    [Library("ttt_equipment_c4")]
    [Buyable(Price = 100)]
    [Equipment(CarriableCategories.OffensiveEquipment, ObjectType = typeof(C4Entity))]
    [Hammer.Skip]
    public partial class C4Equipment : Equipment
    {
        [ServerVar("ttt_c4_can_drop", Help = "If enabled, allows players to drop the C4 as a physics item with Attack2.")]
        public static bool TTTC4CanDrop { get; set; } = false;

        public override string ViewModelPath => "";

        private const int PLACE_DISTANCE = 200;

        public override void Simulate(Client client)
        {
            if (!IsServer || Owner is not Player owner)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1))
                {
                    TraceResult placementTrace = Trace.Ray(Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * PLACE_DISTANCE)
                       .Ignore(owner)
                       .UseHitboxes()
                       .Run();

                    if (!placementTrace.Hit || !placementTrace.Entity.IsWorld)
                    {
                        return;
                    }

                    C4Entity bomb = new();
                    bomb.PhysicsEnabled = false;
                    bomb.Position = placementTrace.EndPosition;
                    bomb.Rotation = Rotation.From(placementTrace.Normal.EulerAngles);
                    bomb.Rotation = bomb.Rotation.RotateAroundAxis(Vector3.Right, -90);
                    bomb.Rotation = bomb.Rotation.RotateAroundAxis(Vector3.Up, 90);

                    owner.Inventory.Remove(this);
                }
                else if (Input.Pressed(InputButton.Attack2) && TTTC4CanDrop)
                {
                    owner.Inventory.DropEntity(this);
                }
            }
        }
    }
}
