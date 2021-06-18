namespace TTTReborn.Items
{
    public interface IBuyableItem
    {
        int GetPrice();

        bool IsBuyable();

        string GetName();
    }
}
