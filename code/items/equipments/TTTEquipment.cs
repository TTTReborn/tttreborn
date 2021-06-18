using Sandbox;

namespace TTTReborn.Items
{
    public class TTTEquipment : Networked, IBuyableItem
    {
        public TTTEquipment()
        {

        }

        public int GetPrice()
        {
            return 100;
        }

        public bool IsBuyable()
        {
            return true;
        }
    }
}
