namespace TTTReborn.Items
{
    public enum SlotType
    {
        Primary = 1,
        Secondary,
        Melee,
        Equipment,
        Grenade
    }

    public interface ICarriableItem : IItem
    {
        SlotType SlotType { get; }

        bool CanDrop();
    }
}
