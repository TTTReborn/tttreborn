using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    /// <summary>
    /// Healthkit equipment definition, for the physical entity, see items/equipments/entities/HealthstationEntity.cs
    /// </summary>
    [Library("ttt_healthstation"), Hammer.Skip]
    public partial class HealthstationEquipment : TTTEquipment, IBuyableItem
    {
        public override string ViewModelPath => "";
        public override SlotType SlotType => SlotType.UtilityEquipment;

        public int Price => 0;

        public HealthstationEquipment() : base()
        {

        }

        public override void Spawn()
        {
            base.Spawn();

            RenderAlpha = 0f;
        }

        public override void Simulate(Client client)
        {
            if (Owner is not TTTPlayer owner && !IsServer)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1))
                {
                    owner.Inventory.DropEntity(this, typeof(HealthstationEntity));
                }
            }
        }

        public override bool CanDrop() => false;
    }
}
