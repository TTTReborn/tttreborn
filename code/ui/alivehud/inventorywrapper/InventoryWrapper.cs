using Sandbox.UI;

namespace TTTReborn.UI
{
    public class InventoryWrapper : Panel
    {
        public Effects Effects;
        public InventorySelection InventorySelection;

        public InventoryWrapper()
        {
            StyleSheet.Load("/ui/alivehud/inventorywrapper/InventoryWrapper.scss");

            Effects = AddChild<Effects>();
            InventorySelection = AddChild<InventorySelection>();
        }
    }
}
