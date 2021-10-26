using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    /// <summary>
    /// Healthkit equipment definition, for the physical entity, see items/equipments/entities/HealthstationEntity.cs
    /// </summary>
    [Library("ttt_healthstation")]
    [Equipment(SlotType = SlotType.UtilityEquipment)]
    [Buyable(Price = 0)]
    [Hammer.Skip]
    public partial class HealthStation : TTTEquipment
    {
        public override string ViewModelPath => "";

        public override void Spawn()
        {
            base.Spawn();

            RenderColor = Color.Transparent;
        }

        public override void Simulate(Client client)
        {
            if (Owner is not TTTPlayer owner || !IsServer)
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
