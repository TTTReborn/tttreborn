using Sandbox;

namespace TTTReborn.Items
{
    public partial class CarriableInfo : ItemInfo
    {
        [Net]
        public CarriableCategories Category { get; set; } = CarriableCategories.SMG;
    }
}
