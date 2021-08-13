using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    /// <summary>
    /// Healthkit equipment definition, for the physical entity, see items/equipments/entities/HealthKitEntity.cs
    /// </summary>
    [Library("ttt_healthkit"), Hammer.Skip]
    public partial class HealthkitEquipment : TTTEquipment, IBuyableItem
    {
        public override string ViewModelPath => "";
        public override SlotType SlotType => SlotType.UtilityEquipment;

        private const int DropPositionOffset = 50;
        private const int DropVelocity = 500;

        public int Price => 0;

        public HealthkitEquipment() : base()
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
                    _ = new HealthkitEntity()
                    {
                        Position = Owner.EyePos + Owner.EyeRot.Forward * DropPositionOffset,
                        Rotation = Owner.EyeRot,
                        Velocity = Owner.EyeRot.Forward * DropVelocity,
                    };
                    (owner.Inventory as Inventory).Remove(this);
                }
            }
        }

        public override bool CanDrop() => false;
    }
}
