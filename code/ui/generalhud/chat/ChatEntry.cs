using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class ChatEntry : Panel
    {
        public string Name;
        public string Text;

        public readonly Sandbox.UI.Panel Message;
        public readonly Image Avatar;
        public readonly Label Header;
        public readonly Label Content;

        public ChatEntry() : base()
        {

            Avatar = Add.Image();
            Avatar.AddClass("avatar");
            Avatar.AddClass("circular");

            Message = Add.Panel("message");

            Header = Message.Add.Label();
            Header.AddClass("header");
            Header.AddClass("text-shadow");

            Content = Message.Add.Label();
            Content.AddClass("content");
            Content.AddClass("text-shadow");
        }
    }
}
