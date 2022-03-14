using Sandbox;

namespace TTTReborn.Items
{
    public partial class ItemInfo : BaseNetworkable
    {
        [Net]
        public string LibraryName { get; set; }

        [Net]
        public Entity Owner { get; set; }
    }
}
