using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    /// <summary>
    /// Decoy equipment definition, for the physical entity, see items/equipments/entities/DecoyEntity.cs
    /// </summary>
    [Library("equipment_decoy")]
    [Weapon(SlotType = SlotType.UtilityEquipment)]
    [Buyable(Price = 0)]
    [Hammer.Skip]
    public partial class DecoyEquipment : TTTEquipment
    {
        public override string ViewModelPath => "";

        public DecoyEquipment() : base()
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
                    owner.Inventory.DropEntity(this, typeof(DecoyEntity));
                }
            }
        }
    }
}
