using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class ChatBoxTextEntry : TextEntry
    {
        public ChatBoxTextEntry(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;
        }

        public override void OnButtonTyped(string button, KeyModifiers km)
        {
            if (button.Equals("tab"))
            {
                ChatBox.Instance.OnTab();

                return;
            }

            base.OnButtonTyped(button, km);
        }
    }
}
