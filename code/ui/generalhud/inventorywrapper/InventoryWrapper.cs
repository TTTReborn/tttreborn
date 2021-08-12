using Sandbox.UI;

namespace TTTReborn.UI
{
    public class InventoryWrapper : TTTPanel
    {
        public static InventoryWrapper Instance;

        public Effects Effects;
        public InventorySelection InventorySelection;

        public InventoryWrapper()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/inventorywrapper/InventoryWrapper.scss");

            Effects = AddChild<Effects>();
            InventorySelection = AddChild<InventorySelection>();
        }
    }
}
