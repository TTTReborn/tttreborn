using Sandbox;

namespace TTTReborn.Items
{
    /// <summary>
    /// Healthkit equipment definition, for the physical entity, see items/equipments/entities/HealthstationEntity.cs
    /// </summary>
    [Library("ttt_equipment_healthstation")]
    [Equipment(CarriableCategories.UtilityEquipment, ObjectType = typeof(HealthstationEntity))]
    [Buyable(Price = 100)]
    [Hammer.Skip]
    public partial class HealthStation : Equipment
    {
        public override string ViewModelPath => "";
    }
}
