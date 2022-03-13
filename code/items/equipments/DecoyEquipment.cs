using Sandbox;

namespace TTTReborn.Items
{
    /// <summary>
    /// Decoy equipment definition, for the physical entity, see items/equipments/entities/DecoyEntity.cs
    /// </summary>
    [Library("ttt_equipment_decoy")]
    [Equipment(CarriableCategories.UtilityEquipment, ObjectType = typeof(DecoyEntity))]
    [Buyable(Price = 100)]
    [Hammer.Skip]
    public partial class DecoyEquipment : Equipment
    {
        public override string ViewModelPath => "";
    }
}
