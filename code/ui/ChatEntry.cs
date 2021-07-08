using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class ChatEntry : Panel
    {
        public readonly Label NameLabel;

        public string Name;

        public string Text;

        public int Index;
        public readonly Panel HeadHolder;
        public readonly Label Message;
        public readonly Image Avatar;

        private readonly RealTimeSince _timeSinceBorn = 0;

        public ChatEntry()
        {
            HeadHolder = Add.Panel("head");
            Avatar = HeadHolder.Add.Image();
            NameLabel = HeadHolder.Add.Label("", "name");
            Message = Add.Label("Message", "message");

        }

        public override void Tick()
        {
            base.Tick();

            if (_timeSinceBorn > 10)
            {
                Delete();
                ChatBox.Instance.Messages.RemoveAt(0);
            }
        }
    }
}
