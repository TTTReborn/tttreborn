namespace TTTReborn.Items
{
    public enum HoldType
    {
        Melee = 1,
        Pistol,
        Primary,
        Heavy,
        Special
    }

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
        HoldType HoldType { get; }
        SlotType SlotType { get; }

        bool CanDrop();
    }
}
