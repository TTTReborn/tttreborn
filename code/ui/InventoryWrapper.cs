using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class InventoryWrapper : Panel
    {
        public Effects Effects;
        public InventorySelection InventorySelection;
        public InventoryWrapper()
        {
            StyleSheet.Load("/ui/InventoryWrapper.scss");

            Effects = AddChild<Effects>();
            InventorySelection = AddChild<InventorySelection>();
        }

    }
}
