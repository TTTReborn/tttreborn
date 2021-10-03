using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    /// <summary>
    /// Decoy equipment definition, for the physical entity, see items/equipments/entities/DecoyEntity.cs
    /// </summary>
    [Library("ttt_decoy"), Hammer.Skip]
    public partial class DecoyEquipment : TTTEquipment, IBuyableItem
    {
        public override string ViewModelPath => "";
        public override SlotType SlotType => SlotType.UtilityEquipment;

        public int Price => 0;

        public DecoyEquipment() : base()
        {

        }

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

            if (Owner is not TTTPlayer owner)
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

        public override bool CanDrop() => false;
    }
}
