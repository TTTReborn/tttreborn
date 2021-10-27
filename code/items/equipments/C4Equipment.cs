using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    /// <summary>
    /// C4 equipment definition, for the physical entity, see items/equipments/entities/C4Entity.cs
    /// </summary>
    [Library("equipment_c4")]
    [Buyable(Price = 0)]
    [Equipment(SlotType = SlotType.OffensiveEquipment)]
    [Hammer.Skip]
    public partial class C4Equipment : TTTEquipment
    {
        [ServerVar("ttt_c4_can_drop", Help = "If enabled, allows players to drop the C4 as a physics item with Attack2.")]
        public static bool TTTC4CanDrop { get; set; } = false;

        public override string ViewModelPath => "";

        private const int PLACE_DISTANCE = 200;

        public C4Equipment() : base()
        {

        }

        public override bool CanDrop() => false;

        public override void Spawn()
        {
            base.Spawn();

            RenderColor = Color.Transparent;
        }

        public override void Simulate(Client client)
        {
            if (!IsServer || Owner is not TTTPlayer owner)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1))
                {
                    TraceResult placementTrace = Trace.Ray(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * PLACE_DISTANCE)
                       .Ignore(owner)
                       .UseHitboxes()
                       .Run();

                    if (!placementTrace.Hit || !placementTrace.Entity.IsWorld)
                    {
                        return;
                    }

                    C4Entity bomb = new C4Entity();
                    bomb.PhysicsEnabled = false;
                    bomb.Position = placementTrace.EndPos;
                    bomb.Rotation = Rotation.From(placementTrace.Normal.EulerAngles);
                    bomb.Rotation = bomb.Rotation.RotateAroundAxis(Vector3.Right, -90);
                    bomb.Rotation = bomb.Rotation.RotateAroundAxis(Vector3.Up, 90);

                    owner.Inventory.Remove(this);
                }
                else if (Input.Pressed(InputButton.Attack2) && TTTC4CanDrop)
                {
                    owner.Inventory.DropEntity(this, typeof(C4Entity));
                }
            }
        }
    }
}
