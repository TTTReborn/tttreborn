using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class ChatEntry : TTTPanel
    {
        public string Name;
        public string Text;

        public readonly Panel HeadHolder;
        public readonly Image Avatar;
        public readonly Label NameLabel;
        public readonly Label Message;

        public ChatEntry()
        {
            HeadHolder = Add.Panel("head");
            Avatar = HeadHolder.Add.Image();
            NameLabel = HeadHolder.Add.Label("", "name");
            Message = Add.Label("Message", "message");
        }
    }
}
