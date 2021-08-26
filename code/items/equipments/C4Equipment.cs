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

        [ServerVar("ttt_c4_can_drop", Help = "If enabled, allows players to drop the C4 as a physics item with Attack2.")]
        public static bool TTTC4CanDrop { get; set; } = false;

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
                if (Input.Pressed(InputButton.Attack1))
                {
                    TraceResult placementTrace = Trace.Ray(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * PlaceDistance)
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
                    bomb.WorldAng = placementTrace.Normal.EulerAngles;
                    bomb.Rotation = bomb.Rotation.RotateAroundAxis(Vector3.Right, -90);
                    bomb.Rotation = bomb.Rotation.RotateAroundAxis(Vector3.Up, 90);

                    (owner.Inventory as Inventory).Remove(this);
                }
                if (Input.Pressed(InputButton.Attack2) && TTTC4CanDrop)
                {
                    (owner.Inventory as Inventory).DropEntity(this, typeof(C4Entity));
                }
            }
        }

        public override bool CanDrop() => false;
    }
}
