using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class ChatEntry : Panel
    {
        public string Name;
        public string Text;

        public readonly Sandbox.UI.Panel HeadHolder;
        public readonly Image Avatar;
        public readonly Label NameLabel;
        public readonly Label Message;

        public ChatEntry() : base()
        {
            HeadHolder = Add.Panel("head");

            Avatar = HeadHolder.Add.Image();
            Avatar.AddClass("avatar");
            Avatar.AddClass("circular");

            NameLabel = HeadHolder.Add.Label();
            NameLabel.AddClass("name");
            NameLabel.AddClass("text-shadow");

            Message = Add.Label();
            Message.AddClass("message");
            Message.AddClass("text-shadow");
        }
    }
}
