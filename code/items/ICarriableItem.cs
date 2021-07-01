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

    public interface ICarriableItem : IItem
    {
        HoldType HoldType { get; }

        bool CanDrop();
    }
}
