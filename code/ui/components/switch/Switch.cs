using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Switch : Sandbox.UI.Switch
    {
        public Switch(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/switch/Switch.scss");
        }
    }
}
