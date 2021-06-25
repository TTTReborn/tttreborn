using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;


namespace TTTReborn.UI
{
    public partial class ChatEntry : Panel
    {
        public readonly Label NameLabel;
        public readonly Label Message;
        public readonly Image Avatar;

        private readonly RealTimeSince _timeSinceBorn = 0;

        public ChatEntry()
        {
            Avatar = Add.Image();
            NameLabel = Add.Label("Name", "name");
            Message = Add.Label("Message", "message");
        }

        public override void Tick()
        {
            base.Tick();

            if (_timeSinceBorn > 10)
            {
                Delete();
            }
        }
    }
}
